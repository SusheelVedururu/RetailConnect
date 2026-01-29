using System.ComponentModel.DataAnnotations;

namespace RetailConnect.API.Models
{
    public class AddCampaignTouchpointRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Campaign ID must be greater than zero")]
        public int CampaignId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Touchpoint ID must be greater than zero")]
        public int TouchpointId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sequence order must be greater than zero")]
        public int SequenceOrder { get; set; } = 1;

        public bool IsActive { get; set; } = true;
    }

    public class CampaignTouchpointResponse
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public int TouchpointId { get; set; }
        public int SequenceOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
