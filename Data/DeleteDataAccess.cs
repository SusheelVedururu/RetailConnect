using System.Data;
using Microsoft.Data.SqlClient;
using RetailConnect.API.Models;
using RetailConnect.API.Data.Helpers;

namespace RetailConnect.API.Data
{
    /// <summary>
    /// Data access layer for delete management operations.
    /// Handles dependency checks, soft delete, hard delete, and restore via stored procedures.
    /// </summary>
    public class DeleteDataAccess
    {
        private readonly string _connectionString;
        private readonly ILogger<DeleteDataAccess> _logger;

        public DeleteDataAccess(IConfiguration configuration, ILogger<DeleteDataAccess> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _logger = logger;
        }

        #region Dependency Check Methods

        /// <summary>
        /// Get dependencies for a Segment (Campaigns that reference it)
        /// </summary>
        public async Task<DeleteDependencyResponse> GetSegmentDependenciesAsync(int segmentId)
        {
            var response = new DeleteDependencyResponse
            {
                EntityId = segmentId,
                EntityType = EntityTypes.Segment
            };

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetSegmentDependencies", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@SegmentId", segmentId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var cache = new OrdinalCache();

            // First result set: Entity info and counts
            if (await reader.ReadAsync())
            {
                var entityExistsOrd = cache.Get(reader, "EntityExists");
                response.EntityExists = reader.GetInt32(entityExistsOrd) == 1;
                if (!response.EntityExists)
                {
                    response.Message = "Segment not found";
                    return response;
                }

                var entityNameOrd = cache.Get(reader, "EntityName");
                var currentStatusOrd = cache.Get(reader, "CurrentStatus");
                var campaignCountOrd = cache.Get(reader, "CampaignCount");

                response.EntityName = reader.IsDBNull(entityNameOrd)
                    ? null
                    : reader.GetString(entityNameOrd);
                response.CurrentStatus = (DeleteStatus)reader.GetByte(currentStatusOrd);

                var campaignCount = reader.GetInt32(campaignCountOrd);
                if (campaignCount > 0)
                {
                    var dep = new DependencyInfo
                    {
                        EntityType = EntityTypes.Campaign,
                        Count = campaignCount
                    };

                    // Move to second result set for sample IDs
                    if (await reader.NextResultAsync())
                    {
                        cache.Clear();
                        var depIdOrd = -1;
                        var depNameOrd = -1;

                        while (await reader.ReadAsync())
                        {
                            if (depIdOrd == -1)
                            {
                                depIdOrd = cache.Get(reader, "DependentId");
                                depNameOrd = cache.Get(reader, "DependentName");
                            }

                            dep.SampleIds.Add(reader.GetInt32(depIdOrd));
                            if (!reader.IsDBNull(depNameOrd))
                                dep.SampleNames.Add(reader.GetString(depNameOrd));
                        }
                    }

                    response.Dependencies.Add(dep);
                    response.TotalDependencyCount = campaignCount;
                }
            }

            SetDependencyResponseFlags(response);
            return response;
        }

        /// <summary>
        /// Get dependencies for a Touchpoint (CampaignTouchpoints that reference it)
        /// </summary>
        public async Task<DeleteDependencyResponse> GetTouchpointDependenciesAsync(int touchpointId)
        {
            var response = new DeleteDependencyResponse
            {
                EntityId = touchpointId,
                EntityType = EntityTypes.Touchpoint
            };

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetTouchpointDependencies", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@TouchpointId", touchpointId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var cache = new OrdinalCache();

            if (await reader.ReadAsync())
            {
                var entityExistsOrd = cache.Get(reader, "EntityExists");
                response.EntityExists = reader.GetInt32(entityExistsOrd) == 1;
                if (!response.EntityExists)
                {
                    response.Message = "Touchpoint not found";
                    return response;
                }

                var entityNameOrd = cache.Get(reader, "EntityName");
                var currentStatusOrd = cache.Get(reader, "CurrentStatus");
                var countOrd = cache.Get(reader, "CampaignTouchpointCount");

                response.EntityName = reader.IsDBNull(entityNameOrd)
                    ? null
                    : reader.GetString(entityNameOrd);
                response.CurrentStatus = (DeleteStatus)reader.GetByte(currentStatusOrd);

                var count = reader.GetInt32(countOrd);
                if (count > 0)
                {
                    var dep = new DependencyInfo
                    {
                        EntityType = EntityTypes.Campaign,
                        Count = count
                    };

                    if (await reader.NextResultAsync())
                    {
                        cache.Clear();
                        var depIdOrd = -1;
                        var depNameOrd = -1;

                        while (await reader.ReadAsync())
                        {
                            if (depIdOrd == -1)
                            {
                                depIdOrd = cache.Get(reader, "DependentId");
                                depNameOrd = cache.Get(reader, "DependentName");
                            }

                            dep.SampleIds.Add(reader.GetInt32(depIdOrd));
                            if (!reader.IsDBNull(depNameOrd))
                                dep.SampleNames.Add(reader.GetString(depNameOrd));
                        }
                    }

                    response.Dependencies.Add(dep);
                    response.TotalDependencyCount = count;
                }
            }

            SetDependencyResponseFlags(response);
            return response;
        }

        /// <summary>
        /// Get dependencies for a Template (CampaignTemplates that reference it)
        /// </summary>
        public async Task<DeleteDependencyResponse> GetTemplateDependenciesAsync(int templateId)
        {
            var response = new DeleteDependencyResponse
            {
                EntityId = templateId,
                EntityType = EntityTypes.Template
            };

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetTemplateDependencies", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@TemplateId", templateId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var cache = new OrdinalCache();

            if (await reader.ReadAsync())
            {
                var entityExistsOrd = cache.Get(reader, "EntityExists");
                response.EntityExists = reader.GetInt32(entityExistsOrd) == 1;
                if (!response.EntityExists)
                {
                    response.Message = "Template not found";
                    return response;
                }

                var entityNameOrd = cache.Get(reader, "EntityName");
                var currentStatusOrd = cache.Get(reader, "CurrentStatus");
                var countOrd = cache.Get(reader, "CampaignTemplateCount");

                response.EntityName = reader.IsDBNull(entityNameOrd)
                    ? null
                    : reader.GetString(entityNameOrd);
                response.CurrentStatus = (DeleteStatus)reader.GetByte(currentStatusOrd);

                var count = reader.GetInt32(countOrd);
                if (count > 0)
                {
                    var dep = new DependencyInfo
                    {
                        EntityType = EntityTypes.Campaign,
                        Count = count
                    };

                    if (await reader.NextResultAsync())
                    {
                        cache.Clear();
                        var depIdOrd = -1;
                        var depNameOrd = -1;

                        while (await reader.ReadAsync())
                        {
                            if (depIdOrd == -1)
                            {
                                depIdOrd = cache.Get(reader, "DependentId");
                                depNameOrd = cache.Get(reader, "DependentName");
                            }

                            dep.SampleIds.Add(reader.GetInt32(depIdOrd));
                            if (!reader.IsDBNull(depNameOrd))
                                dep.SampleNames.Add(reader.GetString(depNameOrd));
                        }
                    }

                    response.Dependencies.Add(dep);
                    response.TotalDependencyCount = count;
                }
            }

            SetDependencyResponseFlags(response);
            return response;
        }

        /// <summary>
        /// Get dependencies for a Campaign (child records - info only, doesn't block delete)
        /// </summary>
        public async Task<DeleteDependencyResponse> GetCampaignDependenciesAsync(int campaignId)
        {
            var response = new DeleteDependencyResponse
            {
                EntityId = campaignId,
                EntityType = EntityTypes.Campaign
            };

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetCampaignDependencies", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CampaignId", campaignId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var cache = new OrdinalCache();

            if (await reader.ReadAsync())
            {
                var entityExistsOrd = cache.Get(reader, "EntityExists");
                response.EntityExists = reader.GetInt32(entityExistsOrd) == 1;
                if (!response.EntityExists)
                {
                    response.Message = "Campaign not found";
                    return response;
                }

                var entityNameOrd = cache.Get(reader, "EntityName");
                var currentStatusOrd = cache.Get(reader, "CurrentStatus");
                var touchpointCountOrd = cache.Get(reader, "TouchpointMappingCount");
                var templateCountOrd = cache.Get(reader, "TemplateMappingCount");
                var logCountOrd = cache.Get(reader, "LogCount");

                response.EntityName = reader.IsDBNull(entityNameOrd)
                    ? null
                    : reader.GetString(entityNameOrd);
                response.CurrentStatus = (DeleteStatus)reader.GetByte(currentStatusOrd);

                // Campaign can always be deleted - these are just informational
                var touchpointCount = reader.GetInt32(touchpointCountOrd);
                var templateCount = reader.GetInt32(templateCountOrd);
                var logCount = reader.GetInt32(logCountOrd);

                if (touchpointCount > 0)
                    response.Dependencies.Add(new DependencyInfo { EntityType = "TouchpointMapping", Count = touchpointCount });
                if (templateCount > 0)
                    response.Dependencies.Add(new DependencyInfo { EntityType = "TemplateMapping", Count = templateCount });
                if (logCount > 0)
                    response.Dependencies.Add(new DependencyInfo { EntityType = "CampaignLog", Count = logCount });

                response.TotalDependencyCount = touchpointCount + templateCount + logCount;
            }

            // Campaign can always be deleted (children are informational)
            response.HasDependencies = response.TotalDependencyCount > 0;
            response.CanSoftDelete = response.EntityExists && response.CurrentStatus == DeleteStatus.Active;
            response.CanHardDelete = response.EntityExists && response.CurrentStatus != DeleteStatus.PendingPurge;
            response.CanRestore = response.EntityExists && response.CurrentStatus == DeleteStatus.SoftDeleted;
            response.Message = BuildDependencyMessage(response, blockHardDelete: false);

            return response;
        }

        #endregion

        #region Delete Operation Methods

        /// <summary>
        /// Perform soft delete on an entity
        /// </summary>
        public async Task<DeleteOperationResult> SoftDeleteEntityAsync(
            string entityType, 
            int entityId, 
            SoftDeleteRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_SoftDeleteEntity", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@EntityType", entityType);
            command.Parameters.AddWithValue("@EntityId", entityId);
            command.Parameters.AddWithValue("@DeletedBy", request.DeletedBy ?? "System");
            command.Parameters.AddWithValue("@DeleteReason", (object?)request.DeleteReason ?? DBNull.Value);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var cache = new OrdinalCache();
                var rowsAffectedOrd = cache.Get(reader, "RowsAffected");
                var newStatusOrd = cache.Get(reader, "NewStatus");
                var messageOrd = cache.Get(reader, "Message");

                return new DeleteOperationResult
                {
                    Success = true,
                    AffectedRecords = reader.GetInt32(rowsAffectedOrd),
                    NewStatus = (DeleteStatus)reader.GetInt32(newStatusOrd),
                    Message = reader.GetString(messageOrd),
                    EntityId = entityId,
                    EntityType = entityType
                };
            }

            return new DeleteOperationResult
            {
                Success = false,
                Message = "No result returned from soft delete operation",
                EntityId = entityId,
                EntityType = entityType
            };
        }

        /// <summary>
        /// Perform hard delete (mark for purge) on an entity
        /// </summary>
        public async Task<DeleteOperationResult> HardDeleteEntityAsync(
            string entityType,
            int entityId,
            HardDeleteRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_HardDeleteEntity", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@EntityType", entityType);
            command.Parameters.AddWithValue("@EntityId", entityId);
            command.Parameters.AddWithValue("@DeletedBy", request.DeletedBy ?? "System");
            command.Parameters.AddWithValue("@DeleteReason", (object?)request.DeleteReason ?? DBNull.Value);
            command.Parameters.AddWithValue("@ForceDelete", request.ForceDelete);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var cache = new OrdinalCache();
                var rowsAffectedOrd = cache.Get(reader, "RowsAffected");
                var newStatusOrd = cache.Get(reader, "NewStatus");
                var messageOrd = cache.Get(reader, "Message");

                return new DeleteOperationResult
                {
                    Success = true,
                    AffectedRecords = reader.GetInt32(rowsAffectedOrd),
                    NewStatus = (DeleteStatus)reader.GetInt32(newStatusOrd),
                    Message = reader.GetString(messageOrd),
                    EntityId = entityId,
                    EntityType = entityType
                };
            }

            return new DeleteOperationResult
            {
                Success = false,
                Message = "No result returned from hard delete operation",
                EntityId = entityId,
                EntityType = entityType
            };
        }

        /// <summary>
        /// Restore a soft-deleted entity
        /// </summary>
        public async Task<DeleteOperationResult> RestoreEntityAsync(string entityType, int entityId, string restoredBy)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_RestoreEntity", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@EntityType", entityType);
            command.Parameters.AddWithValue("@EntityId", entityId);
            command.Parameters.AddWithValue("@RestoredBy", restoredBy);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var cache = new OrdinalCache();
                var rowsAffectedOrd = cache.Get(reader, "RowsAffected");
                var newStatusOrd = cache.Get(reader, "NewStatus");
                var messageOrd = cache.Get(reader, "Message");

                return new DeleteOperationResult
                {
                    Success = true,
                    AffectedRecords = reader.GetInt32(rowsAffectedOrd),
                    NewStatus = (DeleteStatus)reader.GetInt32(newStatusOrd),
                    Message = reader.GetString(messageOrd),
                    EntityId = entityId,
                    EntityType = entityType
                };
            }

            return new DeleteOperationResult
            {
                Success = false,
                Message = "No result returned from restore operation",
                EntityId = entityId,
                EntityType = entityType
            };
        }

        #endregion

        #region Helper Methods

        private static void SetDependencyResponseFlags(DeleteDependencyResponse response)
        {
            response.HasDependencies = response.TotalDependencyCount > 0;
            
            // Soft delete is always allowed for active entities
            response.CanSoftDelete = response.EntityExists && response.CurrentStatus == DeleteStatus.Active;
            
            // Hard delete is blocked if dependencies exist (for non-Campaign entities)
            response.CanHardDelete = response.EntityExists 
                && response.CurrentStatus != DeleteStatus.PendingPurge 
                && !response.HasDependencies;
            
            // Restore only allowed for soft-deleted entities
            response.CanRestore = response.EntityExists && response.CurrentStatus == DeleteStatus.SoftDeleted;

            response.Message = BuildDependencyMessage(response, blockHardDelete: response.HasDependencies);
        }

        private static string BuildDependencyMessage(DeleteDependencyResponse response, bool blockHardDelete)
        {
            if (!response.EntityExists)
                return $"{response.EntityType} not found.";

            if (response.CurrentStatus == DeleteStatus.PendingPurge)
                return $"{response.EntityType} is already marked for permanent deletion.";

            if (response.CurrentStatus == DeleteStatus.SoftDeleted)
                return $"{response.EntityType} is soft deleted. It can be restored or hard deleted.";

            if (!response.HasDependencies)
                return $"{response.EntityType} has no dependencies. Both soft delete and hard delete are allowed.";

            var depSummary = string.Join(", ", response.Dependencies.Select(d => $"{d.Count} {d.EntityType}(s)"));
            
            if (blockHardDelete)
                return $"{response.EntityType} is used by {depSummary}. Soft delete is allowed. Hard delete requires removing dependencies first.";
            
            return $"{response.EntityType} has child records: {depSummary}. These will remain after deletion.";
        }

        #endregion
    }
}
