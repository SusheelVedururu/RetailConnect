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
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    CampaignId = reader.GetInt32(reader.GetOrdinal("CampaignId")),
                    TouchpointId = reader.GetInt32(reader.GetOrdinal("TouchpointId")),
                    SequenceOrder = reader.GetInt32(reader.GetOrdinal("SequenceOrder")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
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
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
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
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    CampaignId = reader.GetInt32(reader.GetOrdinal("CampaignId")),
                    TemplateVersionId = reader.GetInt32(reader.GetOrdinal("TemplateVersionId")),
                    AllocationPercent = (int)reader.GetDecimal(reader.GetOrdinal("AllocationPercent")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
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
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
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
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    CampaignId = reader.GetInt32(reader.GetOrdinal("CampaignId")),
                    ContactId = reader.GetInt32(reader.GetOrdinal("ContactId")),
                    TouchpointId = reader.GetInt32(reader.GetOrdinal("TouchpointId")),
                    TouchCounter = reader.IsDBNull(reader.GetOrdinal("TouchCounter")) ? null : reader.GetInt32(reader.GetOrdinal("TouchCounter")),
                    TemplateVersionId = reader.IsDBNull(reader.GetOrdinal("TemplateVersionId")) ? null : reader.GetInt32(reader.GetOrdinal("TemplateVersionId")),
                    SuccessValue = reader.IsDBNull(reader.GetOrdinal("SuccessValue")) ? null : reader.GetBoolean(reader.GetOrdinal("SuccessValue")),
                    ExecutionStatus = reader.IsDBNull(reader.GetOrdinal("ExecutionStatus")) ? null : reader.GetString(reader.GetOrdinal("ExecutionStatus")),
                    ErrorMessage = reader.IsDBNull(reader.GetOrdinal("ErrorMessage")) ? null : reader.GetString(reader.GetOrdinal("ErrorMessage")),
                    SentDate = reader.IsDBNull(reader.GetOrdinal("SentDate")) ? null : reader.GetDateTime(reader.GetOrdinal("SentDate")),
                    DeliveredDate = reader.IsDBNull(reader.GetOrdinal("DeliveredDate")) ? null : reader.GetDateTime(reader.GetOrdinal("DeliveredDate")),
                    OpenedDate = reader.IsDBNull(reader.GetOrdinal("OpenedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("OpenedDate")),
                    ClickedDate = reader.IsDBNull(reader.GetOrdinal("ClickedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ClickedDate")),
                    LastUpdated = reader.IsDBNull(reader.GetOrdinal("LastUpdated")) ? null : reader.GetDateTime(reader.GetOrdinal("LastUpdated")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                });
            }
            return list;
        }
    }
}
