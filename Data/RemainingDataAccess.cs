using System.Data;
using System.Data.SqlClient;
using RetailConnect.API.Models;

namespace RetailConnect.API.Data
{
    public class CampaignTouchpointDataAccess
    {
        private readonly string _connectionString;

        public CampaignTouchpointDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found.");
        }

        public async Task<int> AddAsync(AddCampaignTouchpointRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_AddCampaignTouchpoint", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CampaignId", request.CampaignId);
            command.Parameters.AddWithValue("@TouchpointId", request.TouchpointId);
            command.Parameters.AddWithValue("@SequenceOrder", request.SequenceOrder);
            command.Parameters.AddWithValue("@IsActive", request.IsActive);
            await connection.OpenAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        public async Task<List<CampaignTouchpointResponse>> GetByCampaignIdAsync(int campaignId)
        {
            var list = new List<CampaignTouchpointResponse>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetCampaignTouchpoints", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CampaignId", campaignId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new CampaignTouchpointResponse
                {
                    Id = reader.GetInt32(0),
                    CampaignId = reader.GetInt32(1),
                    TouchpointId = reader.GetInt32(2),
                    SequenceOrder = reader.GetInt32(3),
                    IsActive = reader.GetBoolean(4),
                    CreatedDate = reader.GetDateTime(5)
                });
            }
            return list;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_RemoveCampaignTouchpoint", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
        }
    }

    public class CampaignTemplateDataAccess
    {
        private readonly string _connectionString;

        public CampaignTemplateDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found.");
        }

        public async Task<int> AddAsync(AddCampaignTemplateRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_AddCampaignTemplate", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CampaignId", request.CampaignId);
            command.Parameters.AddWithValue("@TemplateVersionId", request.TemplateVersionId);
            command.Parameters.AddWithValue("@AllocationPercent", request.AllocationPercent);
            command.Parameters.AddWithValue("@IsActive", request.IsActive);
            await connection.OpenAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        public async Task<List<CampaignTemplateResponse>> GetByCampaignIdAsync(int campaignId)
        {
            var list = new List<CampaignTemplateResponse>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetCampaignTemplates", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CampaignId", campaignId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new CampaignTemplateResponse
                {
                    Id = reader.GetInt32(0),
                    CampaignId = reader.GetInt32(1),
                    TemplateVersionId = reader.GetInt32(2),
                    AllocationPercent = reader.GetInt32(3),
                    IsActive = reader.GetBoolean(4),
                    CreatedDate = reader.GetDateTime(5)
                });
            }
            return list;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_RemoveCampaignTemplate", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
        }
    }

    public class CampaignLogDataAccess
    {
        private readonly string _connectionString;

        public CampaignLogDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found.");
        }

        public async Task<List<CampaignLogResponse>> GetLogsAsync(CampaignLogFilterRequest filter)
        {
            var list = new List<CampaignLogResponse>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetCampaignLogs", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CampaignId", filter.CampaignId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ContactId", filter.ContactId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ExecutionStatus", filter.ExecutionStatus ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@FromDate", filter.FromDate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ToDate", filter.ToDate ?? (object)DBNull.Value);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new CampaignLogResponse
                {
                    Id = reader.GetInt64(0),
                    CampaignId = reader.GetInt32(1),
                    ContactId = reader.GetInt32(2),
                    TouchpointId = reader.GetInt32(3),
                    TouchCounter = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    TemplateVersionId = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                    SuccessValue = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                    ExecutionStatus = reader.IsDBNull(7) ? null : reader.GetString(7),
                    ErrorMessage = reader.IsDBNull(8) ? null : reader.GetString(8),
                    SentDate = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                    DeliveredDate = reader.IsDBNull(10) ? null : reader.GetDateTime(10),
                    OpenedDate = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                    ClickedDate = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                    LastUpdated = reader.IsDBNull(13) ? null : reader.GetDateTime(13),
                    CreatedDate = reader.GetDateTime(14)
                });
            }
            return list;
        }
    }
}
