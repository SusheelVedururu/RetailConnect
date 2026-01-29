using System;
using System.ComponentModel.DataAnnotations;

namespace RetailConnect.API.Models
{
    public class CreateTemplateRequest
    {
        [Required(ErrorMessage = "Template name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(10000, ErrorMessage = "Content cannot exceed 10000 characters")]
        public string? Content { get; set; }

        [MaxLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
        public string? Subject { get; set; } // For email

        [Required(ErrorMessage = "Template type is required")]
        [MaxLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; } = "Email"; // Email, SMS

        public bool IsActive { get; set; } = true;
    }

    public class UpdateTemplateRequest
    {
        [Required(ErrorMessage = "Template name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(10000, ErrorMessage = "Content cannot exceed 10000 characters")]
        public string? Content { get; set; }

        [MaxLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
        public string? Subject { get; set; }

        public bool IsActive { get; set; }
    }

    public class TemplateResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? Subject { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class TemplateListItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
