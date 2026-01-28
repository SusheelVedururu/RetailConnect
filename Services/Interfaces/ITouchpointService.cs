using RetailConnect.API.Models;

namespace RetailConnect.API.Services.Interfaces
{
    public interface ITouchpointService
    {
        Task<TouchpointResponse> CreateTouchpointAsync(CreateTouchpointRequest request);
        Task<TouchpointResponse> GetTouchpointByIdAsync(int id);
        Task<List<TouchpointResponse>> GetAllTouchpointsAsync();
        Task<TouchpointResponse> UpdateTouchpointAsync(int id, UpdateTouchpointRequest request);
    }
}
