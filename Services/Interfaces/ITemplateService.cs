using RetailConnect.API.Models;

namespace RetailConnect.API.Services.Interfaces
{
    public interface ITemplateService
    {
        Task<TemplateResponse> CreateTemplateAsync(CreateTemplateRequest request);
        Task<TemplateResponse> GetTemplateByIdAsync(int id);
        Task<List<TemplateListItem>> GetAllTemplatesAsync();
        Task<TemplateResponse> UpdateTemplateAsync(int id, UpdateTemplateRequest request);
    }
}
