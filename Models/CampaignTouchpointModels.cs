namespace RetailConnect.API.Models
{
    public class AddCampaignTouchpointRequest
    {
        public int CampaignId { get; set; }
        public int TouchpointId { get; set; }
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
