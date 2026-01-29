using System.Data;
using Microsoft.Data.SqlClient;
using RetailConnect.API.Models;
using RetailConnect.API.Data.Helpers;

namespace RetailConnect.API.Data
{
    public class CampaignDataAccess
    {
        private readonly string _connectionString;
        private readonly ILogger<CampaignDataAccess> _logger;

        public CampaignDataAccess(IConfiguration configuration, ILogger<CampaignDataAccess> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _logger = logger;
        }

        public async Task<int> CreateCampaignAsync(CreateCampaignRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_CreateCampaign", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@StartDate", request.StartDate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EndDate", request.EndDate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", request.IsActive);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<CampaignResponse?> GetCampaignByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetCampaignById", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@CampaignId", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var cache = new OrdinalCache();
                var idOrd = cache.Get(reader, "Id");
                var nameOrd = cache.Get(reader, "Name");
                var startDateOrd = cache.Get(reader, "StartDate");
                var endDateOrd = cache.Get(reader, "EndDate");
                var isActiveOrd = cache.Get(reader, "IsActive");
                var createdOrd = cache.Get(reader, "CreatedDate");

                return new CampaignResponse
                {
                    Id = reader.GetInt32(idOrd),
                    Name = reader.GetString(nameOrd),
                    StartDate = reader.IsDBNull(startDateOrd) ? null : reader.GetDateTime(startDateOrd),
                    EndDate = reader.IsDBNull(endDateOrd) ? null : reader.GetDateTime(endDateOrd),
                    IsActive = reader.GetBoolean(isActiveOrd),
                    CreatedDate = reader.GetDateTime(createdOrd)
                };
            }

            return null;
        }

        public async Task<List<CampaignListItem>> GetAllCampaignsAsync()
        {
            var campaigns = new List<CampaignListItem>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetAllCampaigns", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var cache = new OrdinalCache();
            var idOrd = -1;
            var nameOrd = -1;
            var isActiveOrd = -1;

            while (await reader.ReadAsync())
            {
                // Cache ordinals on first read
                if (idOrd == -1)
                {
                    idOrd = cache.Get(reader, "Id");
                    nameOrd = cache.Get(reader, "Name");
                    isActiveOrd = cache.Get(reader, "IsActive");
                }

                campaigns.Add(new CampaignListItem
                {
                    Id = reader.GetInt32(idOrd),
                    Name = reader.GetString(nameOrd),
                    IsActive = reader.GetBoolean(isActiveOrd)
                });
            }

            return campaigns;
        }

        public async Task<bool> UpdateCampaignAsync(int id, UpdateCampaignRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_UpdateCampaign", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@CampaignId", id);
            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@StartDate", request.StartDate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EndDate", request.EndDate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", request.IsActive);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteCampaignAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("DELETE FROM RetailConnect.T_Campaign WHERE CampaignID = @Id", connection);
            command.CommandType = CommandType.Text;

            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> CampaignExistsAsync(string name, int? excludeId = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_CheckCampaignExists", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@ExcludeId", excludeId ?? (object)DBNull.Value);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            var count = result != null ? Convert.ToInt32(result) : 0;

            return count > 0;
        }
    }
}
