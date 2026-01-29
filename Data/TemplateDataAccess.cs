using System.Data;
using Microsoft.Data.SqlClient;
using RetailConnect.API.Models;
using RetailConnect.API.Data.Helpers;

namespace RetailConnect.API.Data
{
    public class TemplateDataAccess
    {
        private readonly string _connectionString;
        private readonly ILogger<TemplateDataAccess> _logger;

        public TemplateDataAccess(IConfiguration configuration, ILogger<TemplateDataAccess> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _logger = logger;
        }

        public async Task<int> CreateTemplateAsync(CreateTemplateRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_CreateTemplate", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@Content", request.Content ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Subject", request.Subject ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Type", request.Type);
            command.Parameters.AddWithValue("@IsActive", request.IsActive);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<TemplateResponse?> GetTemplateByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetTemplateById", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@TemplateId", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var cache = new OrdinalCache();
                var idOrd = cache.Get(reader, "Id");
                var nameOrd = cache.Get(reader, "Name");
                var contentOrd = cache.Get(reader, "Content");
                var subjectOrd = cache.Get(reader, "Subject");
                var typeOrd = cache.Get(reader, "Type");
                var isActiveOrd = cache.Get(reader, "IsActive");
                var createdOrd = cache.Get(reader, "CreatedDate");

                return new TemplateResponse
                {
                    Id = reader.GetInt32(idOrd),
                    Name = reader.GetString(nameOrd),
                    Content = reader.IsDBNull(contentOrd) ? null : reader.GetString(contentOrd),
                    Subject = reader.IsDBNull(subjectOrd) ? null : reader.GetString(subjectOrd),
                    Type = reader.GetString(typeOrd),
                    IsActive = reader.GetBoolean(isActiveOrd),
                    CreatedDate = reader.GetDateTime(createdOrd)
                };
            }

            return null;
        }

        public async Task<List<TemplateListItem>> GetAllTemplatesAsync()
        {
            var templates = new List<TemplateListItem>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetAllTemplates", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var cache = new OrdinalCache();
            var idOrd = -1;
            var nameOrd = -1;
            var typeOrd = -1;
            var isActiveOrd = -1;

            while (await reader.ReadAsync())
            {
                // Cache ordinals on first read
                if (idOrd == -1)
                {
                    idOrd = cache.Get(reader, "Id");
                    nameOrd = cache.Get(reader, "Name");
                    typeOrd = cache.Get(reader, "Type");
                    isActiveOrd = cache.Get(reader, "IsActive");
                }

                templates.Add(new TemplateListItem
                {
                    Id = reader.GetInt32(idOrd),
                    Name = reader.GetString(nameOrd),
                    Type = reader.GetString(typeOrd),
                    IsActive = reader.GetBoolean(isActiveOrd)
                });
            }

            return templates;
        }

        public async Task<bool> UpdateTemplateAsync(int id, UpdateTemplateRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_UpdateTemplate", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@TemplateId", id);
            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@Content", request.Content ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Subject", request.Subject ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", request.IsActive);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }
    }
}
