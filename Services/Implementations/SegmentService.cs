using RetailConnect.API.Data;
using RetailConnect.API.Models;
using RetailConnect.API.Services.Interfaces;

namespace RetailConnect.API.Services.Implementations
{
    /// <summary>
    /// Business logic implementation for Segment operations
    /// Contains ALL validations and business rules
    /// </summary>
    public class SegmentService : ISegmentService
    {
        private readonly SegmentDataAccess _dataAccess;

        public SegmentService(SegmentDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<SegmentResponse> CreateSegmentAsync(CreateSegmentRequest request)
        {
            // Validation: Name is required
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Segment name is required");

            // Validation: Name length
            if (request.Name.Length > 100)
                throw new ArgumentException("Segment name cannot exceed 100 characters");

            // Business Rule: Name must be unique
            var exists = await _dataAccess.SegmentExistsAsync(request.Name);
            if (exists)
                throw new InvalidOperationException($"Segment with name '{request.Name}' already exists");

            // Validation: Criteria format (example business rule)
            if (!string.IsNullOrWhiteSpace(request.Criteria) && !IsValidCriteria(request.Criteria))
                throw new ArgumentException("Invalid segment criteria format");

            // Call data layer
            var segmentId = await _dataAccess.CreateSegmentAsync(request);

            // Retrieve and return the created segment
            var segment = await _dataAccess.GetSegmentByIdAsync(segmentId);
            
            if (segment == null)
                throw new InvalidOperationException("Failed to retrieve created segment");

            return segment;
        }

        public async Task<SegmentResponse> GetSegmentByIdAsync(int id)
        {
            // Validation: ID must be positive
            if (id <= 0)
                throw new ArgumentException("Segment ID must be greater than zero");

            var segment = await _dataAccess.GetSegmentByIdAsync(id);

            // Business Rule: Segment must exist
            if (segment == null)
                throw new KeyNotFoundException($"Segment with ID {id} not found");

            return segment;
        }

        public async Task<List<SegmentListItem>> GetAllSegmentsAsync()
        {
            return await _dataAccess.GetAllSegmentsAsync();
        }

        public async Task<SegmentResponse> UpdateSegmentAsync(int id, UpdateSegmentRequest request)
        {
            // Validation: ID must be positive
            if (id <= 0)
                throw new ArgumentException("Segment ID must be greater than zero");

            // Validation: Name is required
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Segment name is required");

            // Validation: Name length
            if (request.Name.Length > 100)
                throw new ArgumentException("Segment name cannot exceed 100 characters");

            // Business Rule: Segment must exist
            var existingSegment = await _dataAccess.GetSegmentByIdAsync(id);
            if (existingSegment == null)
                throw new KeyNotFoundException($"Segment with ID {id} not found");

            // Business Rule: Name must be unique (if changed)
            if (existingSegment.Name != request.Name)
            {
                var nameExists = await _dataAccess.SegmentExistsAsync(request.Name);
                if (nameExists)
                    throw new InvalidOperationException($"Segment with name '{request.Name}' already exists");
            }

            // Call data layer
            var updated = await _dataAccess.UpdateSegmentAsync(id, request);

            if (!updated)
                throw new InvalidOperationException("Failed to update segment");

            // Retrieve and return the updated segment
            var segment = await _dataAccess.GetSegmentByIdAsync(id);
            
            if (segment == null)
                throw new InvalidOperationException("Failed to retrieve updated segment");

            return segment;
        }

        /// <summary>
        /// Example business rule validation
        /// </summary>
        private bool IsValidCriteria(string criteria)
        {
            // Implement your criteria validation logic here
            // This is just an example
            return !string.IsNullOrWhiteSpace(criteria);
        }
    }
}
