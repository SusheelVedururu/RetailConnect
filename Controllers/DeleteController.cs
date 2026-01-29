using Microsoft.AspNetCore.Mvc;
using RetailConnect.API.Exceptions;
using RetailConnect.API.Models;
using RetailConnect.API.Services.Interfaces;

namespace RetailConnect.API.Controllers
{
    /// <summary>
    /// Centralized Delete Management API.
    /// Handles soft delete, hard delete, restore, and dependency checks for all entities.
    /// </summary>
    [Route("api")]
    [ApiController]
    [Produces("application/json")]
    public class DeleteController : ControllerBase
    {
        private readonly IDeleteService _deleteService;
        private readonly ILogger<DeleteController> _logger;

        public DeleteController(IDeleteService deleteService, ILogger<DeleteController> logger)
        {
            _deleteService = deleteService ?? throw new ArgumentNullException(nameof(deleteService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Dependency Check Endpoints

        /// <summary>
        /// Get delete dependencies for a Campaign.
        /// Shows touchpoint mappings, template mappings, and logs that would be affected.
        /// </summary>
        [HttpGet("campaigns/{id}/delete-dependencies")]
        [ProducesResponseType(typeof(DeleteDependencyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeleteDependencyResponse>> GetCampaignDependencies(int id)
        {
            return await GetDependenciesInternal(EntityTypes.Campaign, id);
        }

        /// <summary>
        /// Get delete dependencies for a Segment.
        /// Shows campaigns that reference this customer segment.
        /// Blocking hard delete if active campaigns use this segment.
        /// </summary>
        [HttpGet("segments/{id}/delete-dependencies")]
        [ProducesResponseType(typeof(DeleteDependencyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeleteDependencyResponse>> GetSegmentDependencies(int id)
        {
            return await GetDependenciesInternal(EntityTypes.Segment, id);
        }

        /// <summary>
        /// Get delete dependencies for a Touchpoint.
        /// Shows campaigns that use this communication channel.
        /// </summary>
        [HttpGet("touchpoints/{id}/delete-dependencies")]
        [ProducesResponseType(typeof(DeleteDependencyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeleteDependencyResponse>> GetTouchpointDependencies(int id)
        {
            return await GetDependenciesInternal(EntityTypes.Touchpoint, id);
        }

        /// <summary>
        /// Get delete dependencies for a Template.
        /// Shows campaigns that use this message template.
        /// </summary>
        [HttpGet("templates/{id}/delete-dependencies")]
        [ProducesResponseType(typeof(DeleteDependencyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeleteDependencyResponse>> GetTemplateDependencies(int id)
        {
            return await GetDependenciesInternal(EntityTypes.Template, id);
        }

        #endregion

        #region Soft Delete Endpoints

        /// <summary>
        /// Soft delete a Campaign.
        /// Campaign remains in database for audit but is hidden from active lists.
        /// </summary>
        [HttpPut("campaigns/{id}/soft-delete")]
        [ProducesResponseType(typeof(DeleteOperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DeleteOperationResult>> SoftDeleteCampaign(int id, [FromBody] SoftDeleteRequest request)
        {
            return await SoftDeleteInternal(EntityTypes.Campaign, id, request);
        }

        /// <summary>
        /// Soft delete a Segment.
        /// Segment remains in database. Active campaigns can continue using it.
        /// </summary>
        [HttpPut("segments/{id}/soft-delete")]
        [ProducesResponseType(typeof(DeleteOperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DeleteOperationResult>> SoftDeleteSegment(int id, [FromBody] SoftDeleteRequest request)
        {
            return await SoftDeleteInternal(EntityTypes.Segment, id, request);
        }

        /// <summary>
        /// Soft delete a Touchpoint.
        /// Communication channel becomes inactive but historical data preserved.
        /// </summary>
        [HttpPut("touchpoints/{id}/soft-delete")]
        [ProducesResponseType(typeof(DeleteOperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DeleteOperationResult>> SoftDeleteTouchpoint(int id, [FromBody] SoftDeleteRequest request)
        {
            return await SoftDeleteInternal(EntityTypes.Touchpoint, id, request);
        }

        /// <summary>
        /// Soft delete a Template.
        /// Message template becomes inactive but historical campaign data preserved.
        /// </summary>
        [HttpPut("templates/{id}/soft-delete")]
        [ProducesResponseType(typeof(DeleteOperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DeleteOperationResult>> SoftDeleteTemplate(int id, [FromBody] SoftDeleteRequest request)
        {
            return await SoftDeleteInternal(EntityTypes.Template, id, request);
        }

        #endregion

        #region Hard Delete Endpoints

        /// <summary>
        /// Hard delete (mark for purge) a Campaign.
        /// Cannot be undone. Record will be permanently removed by purge job.
        /// </summary>
        [HttpDelete("campaigns/{id}/hard-delete")]
        [ProducesResponseType(typeof(DeleteOperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DeleteOperationResult>> HardDeleteCampaign(int id, [FromBody] HardDeleteRequest request)
        {
            return await HardDeleteInternal(EntityTypes.Campaign, id, request);
        }

        /// <summary>
        /// Hard delete (mark for purge) a Segment.
        /// BLOCKED if active campaigns reference this segment (unless ForceDelete=true).
        /// </summary>
        [HttpDelete("segments/{id}/hard-delete")]
        [ProducesResponseType(typeof(DeleteOperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DeleteDependencyResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DeleteOperationResult>> HardDeleteSegment(int id, [FromBody] HardDeleteRequest request)
        {
            return await HardDeleteInternal(EntityTypes.Segment, id, request);
        }

        /// <summary>
        /// Hard delete (mark for purge) a Touchpoint.
        /// BLOCKED if active campaigns use this touchpoint (unless ForceDelete=true).
        /// </summary>
        [HttpDelete("touchpoints/{id}/hard-delete")]
        [ProducesResponseType(typeof(DeleteOperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DeleteDependencyResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DeleteOperationResult>> HardDeleteTouchpoint(int id, [FromBody] HardDeleteRequest request)
        {
            return await HardDeleteInternal(EntityTypes.Touchpoint, id, request);
        }

        /// <summary>
        /// Hard delete (mark for purge) a Template.
        /// BLOCKED if active campaigns use this template (unless ForceDelete=true).
        /// </summary>
        [HttpDelete("templates/{id}/hard-delete")]
        [ProducesResponseType(typeof(DeleteOperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DeleteDependencyResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DeleteOperationResult>> HardDeleteTemplate(int id, [FromBody] HardDeleteRequest request)
        {
            return await HardDeleteInternal(EntityTypes.Template, id, request);
        }

        #endregion

        #region Restore Endpoints

        /// <summary>
        /// Restore a soft-deleted Campaign.
        /// Returns the campaign to active status.
        /// </summary>
        [HttpPut("campaigns/{id}/restore")]
        [ProducesResponseType(typeof(DeleteOperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeleteOperationResult>> RestoreCampaign(int id, [FromQuery] string restoredBy = "System")
        {
            return await RestoreInternal(EntityTypes.Campaign, id, restoredBy);
        }

        /// <summary>
        /// Restore a soft-deleted Segment.
        /// Returns the customer segment to active status.
        /// </summary>
        [HttpPut("segments/{id}/restore")]
        [ProducesResponseType(typeof(DeleteOperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeleteOperationResult>> RestoreSegment(int id, [FromQuery] string restoredBy = "System")
        {
            return await RestoreInternal(EntityTypes.Segment, id, restoredBy);
        }

        /// <summary>
        /// Restore a soft-deleted Touchpoint.
        /// Returns the communication channel to active status.
        /// </summary>
        [HttpPut("touchpoints/{id}/restore")]
        [ProducesResponseType(typeof(DeleteOperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeleteOperationResult>> RestoreTouchpoint(int id, [FromQuery] string restoredBy = "System")
        {
            return await RestoreInternal(EntityTypes.Touchpoint, id, restoredBy);
        }

        /// <summary>
        /// Restore a soft-deleted Template.
        /// Returns the message template to active status.
        /// </summary>
        [HttpPut("templates/{id}/restore")]
        [ProducesResponseType(typeof(DeleteOperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeleteOperationResult>> RestoreTemplate(int id, [FromQuery] string restoredBy = "System")
        {
            return await RestoreInternal(EntityTypes.Template, id, restoredBy);
        }

        #endregion

        #region Private Helper Methods

        private async Task<ActionResult<DeleteDependencyResponse>> GetDependenciesInternal(string entityType, int id)
        {
            try
            {
                var result = await _deleteService.GetDependenciesAsync(entityType, id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidEntityTypeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DeleteOperationFailedException ex)
            {
                _logger.LogError(ex, "Failed to get dependencies");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private async Task<ActionResult<DeleteOperationResult>> SoftDeleteInternal(string entityType, int id, SoftDeleteRequest request)
        {
            try
            {
                // Default DeletedBy if not provided
                request.DeletedBy ??= "System";

                var result = await _deleteService.SoftDeleteAsync(entityType, id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (EntityAlreadyDeletedException ex)
            {
                return Conflict(new { error = ex.Message, currentStatus = ex.CurrentStatus.ToString() });
            }
            catch (DeleteOperationFailedException ex)
            {
                _logger.LogError(ex, "Soft delete failed");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private async Task<ActionResult<DeleteOperationResult>> HardDeleteInternal(string entityType, int id, HardDeleteRequest request)
        {
            try
            {
                // Default DeletedBy if not provided
                request.DeletedBy ??= "System";

                var result = await _deleteService.HardDeleteAsync(entityType, id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (DependencyViolationException ex)
            {
                // Return the dependency info so UI can display it
                return BadRequest(new
                {
                    error = ex.Message,
                    entityType = ex.EntityType,
                    entityId = ex.EntityId,
                    dependencyCount = ex.TotalDependencyCount,
                    dependencies = ex.Dependencies,
                    resolution = "Remove campaign dependencies first, or use ForceDelete=true (not recommended)"
                });
            }
            catch (EntityAlreadyDeletedException ex)
            {
                return Conflict(new { error = ex.Message, currentStatus = ex.CurrentStatus.ToString() });
            }
            catch (DeleteOperationFailedException ex)
            {
                _logger.LogError(ex, "Hard delete failed");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private async Task<ActionResult<DeleteOperationResult>> RestoreInternal(string entityType, int id, string restoredBy)
        {
            try
            {
                var result = await _deleteService.RestoreAsync(entityType, id, restoredBy);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (RestoreNotAllowedException ex)
            {
                return BadRequest(new { error = ex.Message, currentStatus = ex.CurrentStatus.ToString() });
            }
            catch (DeleteOperationFailedException ex)
            {
                _logger.LogError(ex, "Restore failed");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        #endregion
    }
}
