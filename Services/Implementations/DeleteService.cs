using RetailConnect.API.Data;
using RetailConnect.API.Exceptions;
using RetailConnect.API.Models;
using RetailConnect.API.Services.Interfaces;
using System.Data.SqlClient;

namespace RetailConnect.API.Services.Implementations
{
    /// <summary>
    /// Centralized business logic for all delete operations.
    /// Enforces dependency checks, audit logging, and transaction safety.
    /// </summary>
    public class DeleteService : IDeleteService
    {
        private readonly DeleteDataAccess _dataAccess;
        private readonly ILogger<DeleteService> _logger;

        public DeleteService(DeleteDataAccess dataAccess, ILogger<DeleteService> logger)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Dependency Checks

        public async Task<DeleteDependencyResponse> GetDependenciesAsync(string entityType, int entityId)
        {
            // Validate inputs
            ValidateEntityType(entityType);
            ValidateEntityId(entityId);

            _logger.LogInformation("Checking dependencies for {EntityType} ID {EntityId}", entityType, entityId);

            try
            {
                var response = entityType.ToLower() switch
                {
                    "campaign" => await _dataAccess.GetCampaignDependenciesAsync(entityId),
                    "segment" => await _dataAccess.GetSegmentDependenciesAsync(entityId),
                    "touchpoint" => await _dataAccess.GetTouchpointDependenciesAsync(entityId),
                    "template" => await _dataAccess.GetTemplateDependenciesAsync(entityId),
                    _ => throw new InvalidEntityTypeException(entityType)
                };

                if (!response.EntityExists)
                {
                    _logger.LogWarning("{EntityType} ID {EntityId} not found", entityType, entityId);
                    throw new KeyNotFoundException($"{entityType} with ID {entityId} not found");
                }

                _logger.LogInformation(
                    "Dependency check complete for {EntityType} ID {EntityId}: {DependencyCount} dependencies found",
                    entityType, entityId, response.TotalDependencyCount);

                return response;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error checking dependencies for {EntityType} ID {EntityId}", entityType, entityId);
                throw new DeleteOperationFailedException(entityType, entityId, "check dependencies", ex);
            }
        }

        #endregion

        #region Soft Delete

        public async Task<DeleteOperationResult> SoftDeleteAsync(string entityType, int entityId, SoftDeleteRequest request)
        {
            // Validate inputs
            ValidateEntityType(entityType);
            ValidateEntityId(entityId);
            ValidateDeletedBy(request.DeletedBy);

            _logger.LogInformation(
                "Soft delete requested for {EntityType} ID {EntityId} by {DeletedBy}. Reason: {Reason}",
                entityType, entityId, request.DeletedBy, request.DeleteReason ?? "Not specified");

            try
            {
                // Check current state
                var dependencies = await GetDependenciesAsync(entityType, entityId);

                if (!dependencies.CanSoftDelete)
                {
                    if (dependencies.CurrentStatus == DeleteStatus.SoftDeleted)
                        throw new EntityAlreadyDeletedException(entityType, entityId, DeleteStatus.SoftDeleted);
                    if (dependencies.CurrentStatus == DeleteStatus.PendingPurge)
                        throw new EntityAlreadyDeletedException(entityType, entityId, DeleteStatus.PendingPurge);
                }

                // Perform soft delete
                var result = await _dataAccess.SoftDeleteEntityAsync(entityType, entityId, request);

                if (result.Success)
                {
                    _logger.LogInformation(
                        "Successfully soft deleted {EntityType} ID {EntityId}. New status: {NewStatus}",
                        entityType, entityId, result.NewStatus);
                }

                return result;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error during soft delete of {EntityType} ID {EntityId}", entityType, entityId);
                
                // Parse SQL error codes for specific messages
                if (ex.Number == 50001) // Entity not found
                    throw new KeyNotFoundException(ex.Message);
                if (ex.Number == 50002) // Already deleted
                    throw new EntityAlreadyDeletedException(entityType, entityId, DeleteStatus.SoftDeleted);
                
                throw new DeleteOperationFailedException(entityType, entityId, "soft delete", ex);
            }
        }

        #endregion

        #region Hard Delete

        public async Task<DeleteOperationResult> HardDeleteAsync(string entityType, int entityId, HardDeleteRequest request)
        {
            // Validate inputs
            ValidateEntityType(entityType);
            ValidateEntityId(entityId);
            ValidateDeletedBy(request.DeletedBy);

            _logger.LogInformation(
                "Hard delete requested for {EntityType} ID {EntityId} by {DeletedBy}. Force: {Force}. Reason: {Reason}",
                entityType, entityId, request.DeletedBy, request.ForceDelete, request.DeleteReason ?? "Not specified");

            try
            {
                // Check dependencies first
                var dependencies = await GetDependenciesAsync(entityType, entityId);

                // For non-Campaign entities, block hard delete if dependencies exist (unless forced)
                if (dependencies.HasDependencies && !request.ForceDelete && entityType.ToLower() != "campaign")
                {
                    _logger.LogWarning(
                        "Hard delete blocked for {EntityType} ID {EntityId}: {DependencyCount} dependencies exist",
                        entityType, entityId, dependencies.TotalDependencyCount);

                    throw new DependencyViolationException(entityType, entityId, dependencies.Dependencies);
                }

                if (dependencies.CurrentStatus == DeleteStatus.PendingPurge)
                {
                    throw new EntityAlreadyDeletedException(entityType, entityId, DeleteStatus.PendingPurge);
                }

                // Log warning if force deleting with dependencies
                if (dependencies.HasDependencies && request.ForceDelete)
                {
                    _logger.LogWarning(
                        "Force hard delete of {EntityType} ID {EntityId} with {DependencyCount} dependencies",
                        entityType, entityId, dependencies.TotalDependencyCount);
                }

                // Perform hard delete
                var result = await _dataAccess.HardDeleteEntityAsync(entityType, entityId, request);

                if (result.Success)
                {
                    _logger.LogInformation(
                        "Successfully hard deleted {EntityType} ID {EntityId}. New status: {NewStatus}",
                        entityType, entityId, result.NewStatus);
                }

                return result;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error during hard delete of {EntityType} ID {EntityId}", entityType, entityId);

                if (ex.Number == 50001)
                    throw new KeyNotFoundException(ex.Message);
                if (ex.Number == 50002)
                    throw new EntityAlreadyDeletedException(entityType, entityId, DeleteStatus.PendingPurge);
                if (ex.Number == 50004)
                    throw new DependencyViolationException(entityType, entityId, ex.Message);

                throw new DeleteOperationFailedException(entityType, entityId, "hard delete", ex);
            }
        }

        #endregion

        #region Restore

        public async Task<DeleteOperationResult> RestoreAsync(string entityType, int entityId, string restoredBy)
        {
            // Validate inputs
            ValidateEntityType(entityType);
            ValidateEntityId(entityId);
            ValidateDeletedBy(restoredBy);

            _logger.LogInformation(
                "Restore requested for {EntityType} ID {EntityId} by {RestoredBy}",
                entityType, entityId, restoredBy);

            try
            {
                // Check current state
                var dependencies = await GetDependenciesAsync(entityType, entityId);

                if (!dependencies.CanRestore)
                {
                    if (dependencies.CurrentStatus == DeleteStatus.Active)
                        throw new RestoreNotAllowedException(entityType, entityId, DeleteStatus.Active);
                    if (dependencies.CurrentStatus == DeleteStatus.PendingPurge)
                        throw new RestoreNotAllowedException(entityType, entityId, DeleteStatus.PendingPurge);
                }

                // Perform restore
                var result = await _dataAccess.RestoreEntityAsync(entityType, entityId, restoredBy);

                if (result.Success)
                {
                    _logger.LogInformation(
                        "Successfully restored {EntityType} ID {EntityId}. New status: {NewStatus}",
                        entityType, entityId, result.NewStatus);
                }

                return result;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error during restore of {EntityType} ID {EntityId}", entityType, entityId);

                if (ex.Number == 50001)
                    throw new KeyNotFoundException(ex.Message);
                if (ex.Number == 50005)
                    throw new RestoreNotAllowedException(entityType, entityId, DeleteStatus.Active);
                if (ex.Number == 50006)
                    throw new RestoreNotAllowedException(entityType, entityId, DeleteStatus.PendingPurge);

                throw new DeleteOperationFailedException(entityType, entityId, "restore", ex);
            }
        }

        #endregion

        #region Validation Helpers

        private static void ValidateEntityType(string entityType)
        {
            if (string.IsNullOrWhiteSpace(entityType))
                throw new ArgumentException("Entity type is required", nameof(entityType));

            if (!EntityTypes.IsValid(entityType))
                throw new InvalidEntityTypeException(entityType);
        }

        private static void ValidateEntityId(int entityId)
        {
            if (entityId <= 0)
                throw new ArgumentException("Entity ID must be greater than zero", nameof(entityId));
        }

        private static void ValidateDeletedBy(string? deletedBy)
        {
            if (string.IsNullOrWhiteSpace(deletedBy))
                throw new ArgumentException("DeletedBy is required for audit purposes", nameof(deletedBy));
        }

        #endregion
    }
}
