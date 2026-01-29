using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using RetailConnect.API.Models;
using RetailConnect.API.Services.Interfaces;

namespace RetailConnect.API.Controllers
{
    /// <summary>
    /// API endpoints for Segment management
    /// THIN controller - routing only, NO business logic
    /// </summary>
    [ApiController]
    [Route("api/segments")]
    public class SegmentController : ControllerBase
    {
        private readonly ISegmentService _segmentService;

        public SegmentController(ISegmentService segmentService)
        {
            _segmentService = segmentService;
        }

        /// <summary>
        /// Creates a new segment
        /// </summary>
        /// <param name="request">Segment creation details</param>
        /// <returns>Created segment</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SegmentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSegment([FromBody] CreateSegmentRequest request)
        {
            if (request == null)
                return BadRequest("Request body cannot be null");

            try
            {
                var result = await _segmentService.CreateSegmentAsync(request);
                return CreatedAtAction(nameof(GetSegment), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Gets a segment by ID
        /// </summary>
        /// <param name="id">Segment ID</param>
        /// <returns>Segment details</returns>
        [HttpGet("{id}")]
        [OutputCache(PolicyName = "GetById")]
        [ProducesResponseType(typeof(SegmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSegment(int id)
        {
            try
            {
                var result = await _segmentService.GetSegmentByIdAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Gets all segments
        /// </summary>
        /// <returns>List of segments</returns>
        [HttpGet]
        [OutputCache(PolicyName = "GetAll")]
        [ProducesResponseType(typeof(List<SegmentListItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSegments()
        {
            var result = await _segmentService.GetAllSegmentsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing segment
        /// </summary>
        /// <param name="id">Segment ID</param>
        /// <param name="request">Updated segment details</param>
        /// <returns>Updated segment</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SegmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSegment(int id, [FromBody] UpdateSegmentRequest request)
        {
            if (request == null)
                return BadRequest("Request body cannot be null");

            try
            {
                var result = await _segmentService.UpdateSegmentAsync(id, request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSegment(int id)
        {
            try
            {
                var success = await _segmentService.DeleteSegmentAsync(id);
                return success ? Ok() : NotFound();
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        }
    }
}
