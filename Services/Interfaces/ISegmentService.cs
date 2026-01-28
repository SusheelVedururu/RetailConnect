using RetailConnect.API.Models;

namespace RetailConnect.API.Services.Interfaces
{
    /// <summary>
    /// Service contract for Segment business operations
    /// </summary>
    public interface ISegmentService
    {
        Task<SegmentResponse> CreateSegmentAsync(CreateSegmentRequest request);
        Task<SegmentResponse> GetSegmentByIdAsync(int id);
        Task<List<SegmentListItem>> GetAllSegmentsAsync();
        Task<SegmentResponse> UpdateSegmentAsync(int id, UpdateSegmentRequest request);
    }
}
