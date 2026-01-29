using System.ComponentModel.DataAnnotations;

namespace RetailConnect.API.Models
{
    /// <summary>
    /// Request model for creating a new segment
    /// </summary>
    public class CreateSegmentRequest
    {
        [Required(ErrorMessage = "Segment name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [MaxLength(2000, ErrorMessage = "Criteria cannot exceed 2000 characters")]
        public string? Criteria { get; set; }

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Request model for updating an existing segment
    /// </summary>
    public class UpdateSegmentRequest
    {
        [Required(ErrorMessage = "Segment name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [MaxLength(2000, ErrorMessage = "Criteria cannot exceed 2000 characters")]
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
