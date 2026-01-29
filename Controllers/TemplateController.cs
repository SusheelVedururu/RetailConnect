using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using RetailConnect.API.Models;
using RetailConnect.API.Services.Interfaces;

namespace RetailConnect.API.Controllers
{
    [ApiController]
    [Route("api/templates")]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplateController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTemplateRequest request)
        {
            try
            {
                var result = await _templateService.CreateTemplateAsync(request);
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
                var result = await _templateService.GetTemplateByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        }

        [HttpGet]
        [OutputCache(PolicyName = "GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _templateService.GetAllTemplatesAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTemplateRequest request)
        {
            try
            {
                var result = await _templateService.UpdateTemplateAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        }
    }
}
