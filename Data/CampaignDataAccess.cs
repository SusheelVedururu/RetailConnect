using System.Data;
using System.Data.SqlClient;
using RetailConnect.API.Models;

namespace RetailConnect.API.Data
{
    public class CampaignDataAccess
    {
        private readonly string _connectionString;

        public CampaignDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
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
                return new CampaignResponse
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    StartDate = reader.IsDBNull(reader.GetOrdinal("StartDate")) ? null : reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
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

            while (await reader.ReadAsync())
            {
                campaigns.Add(new CampaignListItem
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
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
