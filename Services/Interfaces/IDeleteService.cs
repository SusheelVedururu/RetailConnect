using RetailConnect.API.Models;

namespace RetailConnect.API.Services.Interfaces
{
    /// <summary>
    /// Service contract for centralized delete management operations.
    /// Handles dependency validation, soft delete, hard delete, and restore.
    /// </summary>
    public interface IDeleteService
    {
        #region Dependency Checks

        /// <summary>
        /// Get dependency information for an entity before delete.
        /// Returns what would be affected and whether delete is allowed.
        /// </summary>
        /// <param name="entityType">Type of entity (Campaign, Segment, Touchpoint, Template)</param>
        /// <param name="entityId">ID of the entity</param>
        /// <returns>Dependency analysis with delete options</returns>
        Task<DeleteDependencyResponse> GetDependenciesAsync(string entityType, int entityId);

        #endregion

        #region Delete Operations

        /// <summary>
        /// Perform soft delete on an entity.
        /// Sets DeleteStatus = 1 (SoftDeleted).
        /// Entity remains in database and can be restored.
        /// </summary>
        /// <param name="entityType">Type of entity</param>
        /// <param name="entityId">ID of the entity</param>
        /// <param name="request">Delete request with audit info</param>
        /// <returns>Result of the operation</returns>
        Task<DeleteOperationResult> SoftDeleteAsync(string entityType, int entityId, SoftDeleteRequest request);

        /// <summary>
        /// Perform hard delete on an entity.
        /// Sets DeleteStatus = 2 (PendingPurge).
        /// Cannot be restored. Will be physically removed by purge job.
        /// </summary>
        /// <param name="entityType">Type of entity</param>
        /// <param name="entityId">ID of the entity</param>
        /// <param name="request">Delete request with audit info and force flag</param>
        /// <returns>Result of the operation</returns>
        Task<DeleteOperationResult> HardDeleteAsync(string entityType, int entityId, HardDeleteRequest request);

        /// <summary>
        /// Restore a soft-deleted entity.
        /// Sets DeleteStatus = 0 (Active).
        /// Only works for entities with DeleteStatus = 1.
        /// </summary>
        /// <param name="entityType">Type of entity</param>
        /// <param name="entityId">ID of the entity</param>
        /// <param name="restoredBy">Username performing restore</param>
        /// <returns>Result of the operation</returns>
        Task<DeleteOperationResult> RestoreAsync(string entityType, int entityId, string restoredBy);

        #endregion
    }
}
