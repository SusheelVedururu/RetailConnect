using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using RetailConnect.API.Models;
using RetailConnect.API.Services.Interfaces;

namespace RetailConnect.API.Controllers
{
    [ApiController]
    [Route("api/campaigns")]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CampaignResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignRequest request)
        {
            if (request == null) return BadRequest("Request body cannot be null");

            try
            {
                var result = await _campaignService.CreateCampaignAsync(request);
                return CreatedAtAction(nameof(GetCampaign), new { id = result.Id }, result);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        [HttpGet("{id}")]
        [OutputCache(PolicyName = "GetById")]
        [ProducesResponseType(typeof(CampaignResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCampaign(int id)
        {
            try
            {
                var result = await _campaignService.GetCampaignByIdAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        }

        [HttpGet]
        [OutputCache(PolicyName = "GetAll")]
        [ProducesResponseType(typeof(List<CampaignListItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCampaigns()
        {
            var result = await _campaignService.GetAllCampaignsAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CampaignResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCampaign(int id, [FromBody] UpdateCampaignRequest request)
        {
            if (request == null) return BadRequest("Request body cannot be null");

            try
            {
                var result = await _campaignService.UpdateCampaignAsync(id, request);
                return Ok(result);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCampaign(int id)
        {
            try
            {
                var success = await _campaignService.DeleteCampaignAsync(id);
                return success ? Ok() : NotFound();
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        }
    }
}
