using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using RetailConnect.API.Models;
using RetailConnect.API.Services.Interfaces;

namespace RetailConnect.API.Controllers
{
    [ApiController]
    [Route("api/touchpoints")]
    public class TouchpointController : ControllerBase
    {
        private readonly ITouchpointService _service;

        public TouchpointController(ITouchpointService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTouchpointRequest request)
        {
            try
            {
                var result = await _service.CreateTouchpointAsync(request);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("{id}")]
        [OutputCache(PolicyName = "GetById")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _service.GetTouchpointByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        }

        [HttpGet]
        [OutputCache(PolicyName = "GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllTouchpointsAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTouchpointRequest request)
        {
            try
            {
                var result = await _service.UpdateTouchpointAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteTouchpointAsync(id);
                return success ? Ok() : NotFound();
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        }
    }
}
