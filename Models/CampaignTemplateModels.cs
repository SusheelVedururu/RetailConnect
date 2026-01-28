namespace RetailConnect.API.Models
{
    public class AddCampaignTemplateRequest
    {
        public int CampaignId { get; set; }
        public int TemplateVersionId { get; set; }
        public int AllocationPercent { get; set; } = 100;
        public bool IsActive { get; set; } = true;
    }

    public class CampaignTemplateResponse
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public int TemplateVersionId { get; set; }
        public int AllocationPercent { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
