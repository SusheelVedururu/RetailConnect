namespace RetailConnect.API.Models
{
    /// <summary>
    /// Request model for creating a new segment
    /// </summary>
    public class CreateSegmentRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Criteria { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Request model for updating an existing segment
    /// </summary>
    public class UpdateSegmentRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Criteria { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Response model for segment operations
    /// </summary>
    public class SegmentResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Criteria { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    /// <summary>
    /// Lightweight segment list item
    /// </summary>
    public class SegmentListItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int MemberCount { get; set; }
    }
}
