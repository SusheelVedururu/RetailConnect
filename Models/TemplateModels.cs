using System;

namespace RetailConnect.API.Models
{
    public class CreateTemplateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? Subject { get; set; } // For email
        public string Type { get; set; } = "Email"; // Email, SMS
        public bool IsActive { get; set; } = true;
    }

    public class UpdateTemplateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Content { get; set; }
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
