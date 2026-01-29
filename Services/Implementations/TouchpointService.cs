using RetailConnect.API.Data;
using RetailConnect.API.Models;
using RetailConnect.API.Services.Interfaces;

namespace RetailConnect.API.Services.Implementations
{
    public class TouchpointService : ITouchpointService
    {
        private readonly TouchpointDataAccess _dataAccess;
        private readonly ILogger<TouchpointService> _logger;

        public TouchpointService(TouchpointDataAccess dataAccess, ILogger<TouchpointService> logger)
        {
            _dataAccess = dataAccess;
            _logger = logger;
        }

        public async Task<TouchpointResponse> CreateTouchpointAsync(CreateTouchpointRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Name is required");
            if (string.IsNullOrWhiteSpace(request.Type)) throw new ArgumentException("Type is required");

            var id = await _dataAccess.CreateTouchpointAsync(request);
            var item = await _dataAccess.GetTouchpointByIdAsync(id);
            return item ?? throw new InvalidOperationException("Failed to create touchpoint");
        }

        public async Task<TouchpointResponse> GetTouchpointByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid ID");
            var item = await _dataAccess.GetTouchpointByIdAsync(id);
            return item ?? throw new KeyNotFoundException($"Touchpoint {id} not found");
        }

        public async Task<List<TouchpointResponse>> GetAllTouchpointsAsync()
        {
            return await _dataAccess.GetAllTouchpointsAsync();
        }

        public async Task<TouchpointResponse> UpdateTouchpointAsync(int id, UpdateTouchpointRequest request)
        {
            if (id <= 0) throw new ArgumentException("Invalid ID");
            if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Name is required");

            var updated = await _dataAccess.UpdateTouchpointAsync(id, request);
            if (!updated) throw new InvalidOperationException("Update failed");

            return await GetTouchpointByIdAsync(id);
        }
        public async Task<bool> DeleteTouchpointAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid ID");
            var exists = await _dataAccess.GetTouchpointByIdAsync(id);
            if (exists == null) throw new KeyNotFoundException($"Touchpoint {id} not found");

            return await _dataAccess.DeleteTouchpointAsync(id);
        }
    }
}
