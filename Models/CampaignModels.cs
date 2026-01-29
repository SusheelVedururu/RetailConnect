using System;
using System.ComponentModel.DataAnnotations;

namespace RetailConnect.API.Models
{
    /// <summary>
    /// Request to create a new campaign
    /// </summary>
    public class CreateCampaignRequest
    {
        [Required(ErrorMessage = "Campaign name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Request to update a campaign
    /// </summary>
    public class UpdateCampaignRequest
    {
        [Required(ErrorMessage = "Campaign name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Response model for a campaign
    /// </summary>
    public class CampaignResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    
    /// <summary>
    /// List item model for campaigns
    /// </summary>
    public class CampaignListItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
