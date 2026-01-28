using RetailConnect.API.Data;
using RetailConnect.API.Models;
using RetailConnect.API.Services.Interfaces;

namespace RetailConnect.API.Services.Implementations
{
    public class TemplateService : ITemplateService
    {
        private readonly TemplateDataAccess _dataAccess;

        public TemplateService(TemplateDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<TemplateResponse> CreateTemplateAsync(CreateTemplateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Template name is required");
            if (string.IsNullOrWhiteSpace(request.Type))
                throw new ArgumentException("Template type is required");

            var templateId = await _dataAccess.CreateTemplateAsync(request);
            var template = await _dataAccess.GetTemplateByIdAsync(templateId);

            return template ?? throw new InvalidOperationException("Failed to retrieve created template");
        }

        public async Task<TemplateResponse> GetTemplateByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid template ID");
            var template = await _dataAccess.GetTemplateByIdAsync(id);
            return template ?? throw new KeyNotFoundException($"Template {id} not found");
        }

        public async Task<List<TemplateListItem>> GetAllTemplatesAsync()
        {
            return await _dataAccess.GetAllTemplatesAsync();
        }

        public async Task<TemplateResponse> UpdateTemplateAsync(int id, UpdateTemplateRequest request)
        {
            if (id <= 0) throw new ArgumentException("Invalid template ID");
            if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Template name is required");

            var updated = await _dataAccess.UpdateTemplateAsync(id, request);
            if (!updated) throw new InvalidOperationException("Failed to update template");

            return await GetTemplateByIdAsync(id);
        }
    }
}
