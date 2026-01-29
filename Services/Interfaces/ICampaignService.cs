using RetailConnect.API.Models;

namespace RetailConnect.API.Services.Interfaces
{
    public interface ICampaignService
    {
        Task<CampaignResponse> CreateCampaignAsync(CreateCampaignRequest request);
        Task<CampaignResponse> GetCampaignByIdAsync(int id);
        Task<List<CampaignListItem>> GetAllCampaignsAsync();
        Task<CampaignResponse> UpdateCampaignAsync(int id, UpdateCampaignRequest request);
        Task<bool> DeleteCampaignAsync(int id);
    }
}
