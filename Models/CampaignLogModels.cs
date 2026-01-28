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
        public int? SuccessValue { get; set; }
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
        public int? CampaignId { get; set; }
        public int? ContactId { get; set; }
        public string? ExecutionStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
