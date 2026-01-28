namespace RetailConnect.API.Models
{
    public class CreateTouchpointRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // EmailProvider, SMSGateway
        public string? Configuration { get; set; } // JSON or connection string
        public bool IsActive { get; set; } = true;
    }

    public class UpdateTouchpointRequest
    {
        public string Name { get; set; } = string.Empty;
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
