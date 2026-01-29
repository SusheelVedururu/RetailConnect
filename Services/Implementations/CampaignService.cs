using RetailConnect.API.Data;
using RetailConnect.API.Models;
using RetailConnect.API.Services.Interfaces;

namespace RetailConnect.API.Services.Implementations
{
    public class CampaignService : ICampaignService
    {
        private readonly CampaignDataAccess _dataAccess;

        public CampaignService(CampaignDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<CampaignResponse> CreateCampaignAsync(CreateCampaignRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Campaign name is required");

            if (request.StartDate.HasValue && request.EndDate.HasValue && request.EndDate < request.StartDate)
                throw new ArgumentException("End date cannot be before start date");

            var exists = await _dataAccess.CampaignExistsAsync(request.Name);
            if (exists)
                throw new InvalidOperationException($"Campaign with name '{request.Name}' already exists");

            var campaignId = await _dataAccess.CreateCampaignAsync(request);
            var campaign = await _dataAccess.GetCampaignByIdAsync(campaignId);

            if (campaign == null)
                throw new InvalidOperationException("Failed to retrieve created campaign");

            return campaign;
        }

        public async Task<CampaignResponse> GetCampaignByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Campaign ID must be greater than zero");

            var campaign = await _dataAccess.GetCampaignByIdAsync(id);

            if (campaign == null)
                throw new KeyNotFoundException($"Campaign with ID {id} not found");

            return campaign;
        }

        public async Task<List<CampaignListItem>> GetAllCampaignsAsync()
        {
            return await _dataAccess.GetAllCampaignsAsync();
        }

        public async Task<CampaignResponse> UpdateCampaignAsync(int id, UpdateCampaignRequest request)
        {
            if (id <= 0)
                throw new ArgumentException("Campaign ID must be greater than zero");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Campaign name is required");

            var existingCampaign = await _dataAccess.GetCampaignByIdAsync(id);
            if (existingCampaign == null)
                throw new KeyNotFoundException($"Campaign with ID {id} not found");

            if (existingCampaign.Name != request.Name)
            {
                var exists = await _dataAccess.CampaignExistsAsync(request.Name, id);
                if (exists)
                    throw new InvalidOperationException($"Campaign with name '{request.Name}' already exists");
            }

            var updated = await _dataAccess.UpdateCampaignAsync(id, request);
            if (!updated)
                throw new InvalidOperationException("Failed to update campaign");

            var campaign = await _dataAccess.GetCampaignByIdAsync(id);
            if (campaign == null)
                throw new InvalidOperationException("Failed to retrieve updated campaign");

            return campaign;
        }
        public async Task<bool> DeleteCampaignAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid ID");
            var exists = await _dataAccess.GetCampaignByIdAsync(id);
            if (exists == null) throw new KeyNotFoundException($"Campaign {id} not found");

            return await _dataAccess.DeleteCampaignAsync(id);
        }
    }
}
