using RetailConnect.API.Models;

namespace RetailConnect.API.Exceptions
{
    /// <summary>
    /// Base exception for all delete-related errors
    /// </summary>
    public abstract class DeleteException : Exception
    {
        public string EntityType { get; }
        public int EntityId { get; }

        protected DeleteException(string message, string entityType, int entityId) 
            : base(message)
        {
            EntityType = entityType;
            EntityId = entityId;
        }

        protected DeleteException(string message, string entityType, int entityId, Exception innerException)
            : base(message, innerException)
        {
            EntityType = entityType;
            EntityId = entityId;
        }
    }

    /// <summary>
    /// Thrown when attempting to delete an entity that has active dependencies
    /// </summary>
    public class DependencyViolationException : DeleteException
    {
        public List<DependencyInfo> Dependencies { get; }
        public int TotalDependencyCount { get; }

        public DependencyViolationException(
            string entityType, 
            int entityId, 
            List<DependencyInfo> dependencies)
            : base(
                $"Cannot delete {entityType} with ID {entityId}: {dependencies.Sum(d => d.Count)} active dependencies exist.",
                entityType,
                entityId)
        {
            Dependencies = dependencies;
            TotalDependencyCount = dependencies.Sum(d => d.Count);
        }

        public DependencyViolationException(
            string entityType,
            int entityId,
            string dependencyDescription)
            : base(
                $"Cannot delete {entityType} with ID {entityId}: {dependencyDescription}",
                entityType,
                entityId)
        {
            Dependencies = new List<DependencyInfo>();
            TotalDependencyCount = 0;
        }
    }

    /// <summary>
    /// Thrown when attempting to delete an entity that is already deleted
    /// </summary>
    public class EntityAlreadyDeletedException : DeleteException
    {
        public DeleteStatus CurrentStatus { get; }

        public EntityAlreadyDeletedException(string entityType, int entityId, DeleteStatus currentStatus)
            : base(
                $"{entityType} with ID {entityId} is already {(currentStatus == DeleteStatus.SoftDeleted ? "soft deleted" : "marked for purge")}.",
                entityType,
                entityId)
        {
            CurrentStatus = currentStatus;
        }
    }

    /// <summary>
    /// Thrown when attempting to restore an entity that cannot be restored
    /// </summary>
    public class RestoreNotAllowedException : DeleteException
    {
        public DeleteStatus CurrentStatus { get; }

        public RestoreNotAllowedException(string entityType, int entityId, DeleteStatus currentStatus)
            : base(
                currentStatus == DeleteStatus.PendingPurge
                    ? $"Cannot restore {entityType} with ID {entityId}: Entity is marked for permanent deletion."
                    : $"Cannot restore {entityType} with ID {entityId}: Entity is already active.",
                entityType,
                entityId)
        {
            CurrentStatus = currentStatus;
        }
    }

    /// <summary>
    /// Thrown when the entity type provided is not valid
    /// </summary>
    public class InvalidEntityTypeException : Exception
    {
        public string ProvidedEntityType { get; }

        public InvalidEntityTypeException(string providedEntityType)
            : base($"Invalid entity type: '{providedEntityType}'. Valid types are: {string.Join(", ", EntityTypes.All)}")
        {
            ProvidedEntityType = providedEntityType;
        }
    }

    /// <summary>
    /// Thrown when a delete operation fails at the database level
    /// </summary>
    public class DeleteOperationFailedException : DeleteException
    {
        public string Operation { get; }

        public DeleteOperationFailedException(string entityType, int entityId, string operation, Exception innerException)
            : base(
                $"Failed to {operation} {entityType} with ID {entityId}: {innerException.Message}",
                entityType,
                entityId,
                innerException)
        {
            Operation = operation;
        }
    }
}
