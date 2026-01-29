using System.ComponentModel.DataAnnotations;

namespace RetailConnect.API.Models
{
    /// <summary>
    /// Delete status enumeration for soft/hard delete management
    /// </summary>
    public enum DeleteStatus
    {
        /// <summary>Normal, visible record</summary>
        Active = 0,
        
        /// <summary>Logically deleted, retained for audit/recovery</summary>
        SoftDeleted = 1,
        
        /// <summary>Marked for physical deletion by purge job</summary>
        PendingPurge = 2
    }

    /// <summary>
    /// Supported entity types for delete operations
    /// </summary>
    public static class EntityTypes
    {
        public const string Campaign = "Campaign";
        public const string Segment = "Segment";
        public const string Touchpoint = "Touchpoint";
        public const string Template = "Template";

        public static readonly string[] All = { Campaign, Segment, Touchpoint, Template };

        public static bool IsValid(string entityType) =>
            All.Contains(entityType, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Request model for soft delete operation
    /// </summary>
    public class SoftDeleteRequest
    {
        /// <summary>Username or system performing the delete</summary>
        [Required(ErrorMessage = "DeletedBy is required")]
        [MaxLength(100, ErrorMessage = "DeletedBy cannot exceed 100 characters")]
        public string DeletedBy { get; set; } = "System";

        /// <summary>Optional reason for deletion (audit purposes)</summary>
        [MaxLength(500, ErrorMessage = "Delete reason cannot exceed 500 characters")]
        public string? DeleteReason { get; set; }
    }

    /// <summary>
    /// Request model for hard delete operation
    /// </summary>
    public class HardDeleteRequest
    {
        /// <summary>Username or system performing the delete</summary>
        [Required(ErrorMessage = "DeletedBy is required")]
        [MaxLength(100, ErrorMessage = "DeletedBy cannot exceed 100 characters")]
        public string DeletedBy { get; set; } = "System";

        /// <summary>Optional reason for deletion (audit purposes)</summary>
        [MaxLength(500, ErrorMessage = "Delete reason cannot exceed 500 characters")]
        public string? DeleteReason { get; set; }

        /// <summary>
        /// Set to true to force delete even if dependencies exist.
        /// This will NOT delete dependencies, just override the check.
        /// Use with extreme caution.
        /// </summary>
        public bool ForceDelete { get; set; } = false;
    }

    /// <summary>
    /// Information about a single dependency type
    /// </summary>
    public class DependencyInfo
    {
        /// <summary>Type of dependent entity (e.g., "Campaign")</summary>
        public string EntityType { get; set; } = string.Empty;
        
        /// <summary>Number of dependencies of this type</summary>
        public int Count { get; set; }
        
        /// <summary>Sample IDs of dependent entities (max 10)</summary>
        public List<int> SampleIds { get; set; } = new();
        
        /// <summary>Sample names of dependent entities (max 10)</summary>
        public List<string> SampleNames { get; set; } = new();
    }

    /// <summary>
    /// Response model for dependency check operation
    /// </summary>
    public class DeleteDependencyResponse
    {
        /// <summary>ID of the entity being checked</summary>
        public int EntityId { get; set; }
        
        /// <summary>Type of entity (Campaign, Segment, etc.)</summary>
        public string EntityType { get; set; } = string.Empty;
        
        /// <summary>Name of the entity</summary>
        public string? EntityName { get; set; }
        
        /// <summary>Current delete status of the entity</summary>
        public DeleteStatus CurrentStatus { get; set; }
        
        /// <summary>Whether the entity exists in the database</summary>
        public bool EntityExists { get; set; }
        
        /// <summary>Whether any active dependencies exist</summary>
        public bool HasDependencies { get; set; }
        
        /// <summary>Total count of all dependencies</summary>
        public int TotalDependencyCount { get; set; }
        
        /// <summary>Whether soft delete is allowed</summary>
        public bool CanSoftDelete { get; set; }
        
        /// <summary>Whether hard delete is allowed (no dependencies or force allowed)</summary>
        public bool CanHardDelete { get; set; }
        
        /// <summary>Whether restore is allowed (only if soft deleted)</summary>
        public bool CanRestore { get; set; }
        
        /// <summary>List of dependency details by type</summary>
        public List<DependencyInfo> Dependencies { get; set; } = new();
        
        /// <summary>Human-readable message explaining the situation</summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Result of a delete or restore operation
    /// </summary>
    public class DeleteOperationResult
    {
        /// <summary>Whether the operation succeeded</summary>
        public bool Success { get; set; }
        
        /// <summary>Human-readable result message</summary>
        public string Message { get; set; } = string.Empty;
        
        /// <summary>New status after the operation</summary>
        public DeleteStatus NewStatus { get; set; }
        
        /// <summary>Number of records affected</summary>
        public int AffectedRecords { get; set; }
        
        /// <summary>ID of the affected entity</summary>
        public int EntityId { get; set; }
        
        /// <summary>Type of the affected entity</summary>
        public string EntityType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Audit log entry for delete operations
    /// </summary>
    public class DeleteAuditLogEntry
    {
        public long AuditLogId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string? EntityName { get; set; }
        public string DeleteAction { get; set; } = string.Empty;
        public int PreviousStatus { get; set; }
        public int NewStatus { get; set; }
        public string DeletedBy { get; set; } = string.Empty;
        public string? DeleteReason { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
