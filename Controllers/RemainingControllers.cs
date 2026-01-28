using Microsoft.AspNetCore.Mvc;
using RetailConnect.API.Data;
using RetailConnect.API.Models;

namespace RetailConnect.API.Controllers
{
    [ApiController]
    [Route("api/campaigns/{campaignId}/touchpoints")]
    public class CampaignTouchpointController : ControllerBase
    {
        private readonly CampaignTouchpointDataAccess _dataAccess;

        public CampaignTouchpointController(CampaignTouchpointDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int campaignId, [FromBody] AddCampaignTouchpointRequest request)
        {
            request.CampaignId = campaignId;
            var id = await _dataAccess.AddAsync(request);
            return Ok(new { id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int campaignId)
        {
            var result = await _dataAccess.GetByCampaignIdAsync(campaignId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var success = await _dataAccess.RemoveAsync(id);
            return success ? Ok() : NotFound();
        }
    }

    [ApiController]
    [Route("api/campaigns/{campaignId}/templates")]
    public class CampaignTemplateController : ControllerBase
    {
        private readonly CampaignTemplateDataAccess _dataAccess;

        public CampaignTemplateController(CampaignTemplateDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int campaignId, [FromBody] AddCampaignTemplateRequest request)
        {
            request.CampaignId = campaignId;
            var id = await _dataAccess.AddAsync(request);
            return Ok(new { id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int campaignId)
        {
            var result = await _dataAccess.GetByCampaignIdAsync(campaignId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var success = await _dataAccess.RemoveAsync(id);
            return success ? Ok() : NotFound();
        }
    }

    [ApiController]
    [Route("api/campaign-logs")]
    public class CampaignLogController : ControllerBase
    {
        private readonly CampaignLogDataAccess _dataAccess;

        public CampaignLogController(CampaignLogDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] CampaignLogFilterRequest filter)
        {
            var result = await _dataAccess.GetLogsAsync(filter);
            return Ok(result);
        }
    }
}
