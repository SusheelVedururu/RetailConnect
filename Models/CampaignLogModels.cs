using System.ComponentModel.DataAnnotations;

namespace RetailConnect.API.Models
{
    public class CampaignLogResponse
    {
        public long Id { get; set; }
        public int CampaignId { get; set; }
        public int ContactId { get; set; }
        public int TouchpointId { get; set; }
        public int? TouchCounter { get; set; }
        public int? TemplateVersionId { get; set; }
        public bool? SuccessValue { get; set; }
        public string? ExecutionStatus { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? OpenedDate { get; set; }
        public DateTime? ClickedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CampaignLogFilterRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Campaign ID must be greater than zero")]
        public int? CampaignId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Contact ID must be greater than zero")]
        public int? ContactId { get; set; }

        [MaxLength(50, ErrorMessage = "Execution status cannot exceed 50 characters")]
        public string? ExecutionStatus { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
