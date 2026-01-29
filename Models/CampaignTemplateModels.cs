using System.ComponentModel.DataAnnotations;

namespace RetailConnect.API.Models
{
    public class AddCampaignTemplateRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Campaign ID must be greater than zero")]
        public int CampaignId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Template version ID must be greater than zero")]
        public int TemplateVersionId { get; set; }

        [Range(0, 100, ErrorMessage = "Allocation percent must be between 0 and 100")]
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
