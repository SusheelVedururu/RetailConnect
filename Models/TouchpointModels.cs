using System.ComponentModel.DataAnnotations;

namespace RetailConnect.API.Models
{
    public class CreateTouchpointRequest
    {
        [Required(ErrorMessage = "Touchpoint name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Touchpoint type is required")]
        [MaxLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; } = string.Empty; // EmailProvider, SMSGateway

        [MaxLength(5000, ErrorMessage = "Configuration cannot exceed 5000 characters")]
        public string? Configuration { get; set; } // JSON or connection string

        public bool IsActive { get; set; } = true;
    }

    public class UpdateTouchpointRequest
    {
        [Required(ErrorMessage = "Touchpoint name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(5000, ErrorMessage = "Configuration cannot exceed 5000 characters")]
        public string? Configuration { get; set; }

        public bool IsActive { get; set; }
    }

    public class TouchpointResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Configuration { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
