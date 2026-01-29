using System.Data;
using Microsoft.Data.SqlClient;
using RetailConnect.API.Models;
using RetailConnect.API.Data.Helpers;

namespace RetailConnect.API.Data
{
    public class CampaignTouchpointDataAccess
    {
        private readonly string _connectionString;
        private readonly ILogger<CampaignTouchpointDataAccess> _logger;

        public CampaignTouchpointDataAccess(IConfiguration configuration, ILogger<CampaignTouchpointDataAccess> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found.");
            _logger = logger;
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

            var cache = new OrdinalCache();
            var idOrd = -1;
            var campaignIdOrd = -1;
            var touchpointIdOrd = -1;
            var sequenceOrderOrd = -1;
            var isActiveOrd = -1;
            var createdDateOrd = -1;

            while (await reader.ReadAsync())
            {
                // Cache ordinals on first read
                if (idOrd == -1)
                {
                    idOrd = cache.Get(reader, "Id");
                    campaignIdOrd = cache.Get(reader, "CampaignId");
                    touchpointIdOrd = cache.Get(reader, "TouchpointId");
                    sequenceOrderOrd = cache.Get(reader, "SequenceOrder");
                    isActiveOrd = cache.Get(reader, "IsActive");
                    createdDateOrd = cache.Get(reader, "CreatedDate");
                }

                list.Add(new CampaignTouchpointResponse
                {
                    Id = reader.GetInt32(idOrd),
                    CampaignId = reader.GetInt32(campaignIdOrd),
                    TouchpointId = reader.GetInt32(touchpointIdOrd),
                    SequenceOrder = reader.GetInt32(sequenceOrderOrd),
                    IsActive = reader.GetBoolean(isActiveOrd),
                    CreatedDate = reader.GetDateTime(createdDateOrd)
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
        private readonly ILogger<CampaignTemplateDataAccess> _logger;

        public CampaignTemplateDataAccess(IConfiguration configuration, ILogger<CampaignTemplateDataAccess> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found.");
            _logger = logger;
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

            var cache = new OrdinalCache();
            var idOrd = -1;
            var campaignIdOrd = -1;
            var templateVersionIdOrd = -1;
            var allocationPercentOrd = -1;
            var isActiveOrd = -1;
            var createdDateOrd = -1;

            while (await reader.ReadAsync())
            {
                // Cache ordinals on first read
                if (idOrd == -1)
                {
                    idOrd = cache.Get(reader, "Id");
                    campaignIdOrd = cache.Get(reader, "CampaignId");
                    templateVersionIdOrd = cache.Get(reader, "TemplateVersionId");
                    allocationPercentOrd = cache.Get(reader, "AllocationPercent");
                    isActiveOrd = cache.Get(reader, "IsActive");
                    createdDateOrd = cache.Get(reader, "CreatedDate");
                }

                list.Add(new CampaignTemplateResponse
                {
                    Id = reader.GetInt32(idOrd),
                    CampaignId = reader.GetInt32(campaignIdOrd),
                    TemplateVersionId = reader.GetInt32(templateVersionIdOrd),
                    AllocationPercent = (int)reader.GetDecimal(allocationPercentOrd),
                    IsActive = reader.GetBoolean(isActiveOrd),
                    CreatedDate = reader.GetDateTime(createdDateOrd)
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
        private readonly ILogger<CampaignLogDataAccess> _logger;

        public CampaignLogDataAccess(IConfiguration configuration, ILogger<CampaignLogDataAccess> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found.");
            _logger = logger;
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

            var cache = new OrdinalCache();
            var idOrd = -1;
            var campaignIdOrd = -1;
            var contactIdOrd = -1;
            var touchpointIdOrd = -1;
            var touchCounterOrd = -1;
            var templateVersionIdOrd = -1;
            var successValueOrd = -1;
            var executionStatusOrd = -1;
            var errorMessageOrd = -1;
            var sentDateOrd = -1;
            var deliveredDateOrd = -1;
            var openedDateOrd = -1;
            var clickedDateOrd = -1;
            var lastUpdatedOrd = -1;
            var createdDateOrd = -1;

            while (await reader.ReadAsync())
            {
                // Cache ordinals on first read
                if (idOrd == -1)
                {
                    idOrd = cache.Get(reader, "Id");
                    campaignIdOrd = cache.Get(reader, "CampaignId");
                    contactIdOrd = cache.Get(reader, "ContactId");
                    touchpointIdOrd = cache.Get(reader, "TouchpointId");
                    touchCounterOrd = cache.Get(reader, "TouchCounter");
                    templateVersionIdOrd = cache.Get(reader, "TemplateVersionId");
                    successValueOrd = cache.Get(reader, "SuccessValue");
                    executionStatusOrd = cache.Get(reader, "ExecutionStatus");
                    errorMessageOrd = cache.Get(reader, "ErrorMessage");
                    sentDateOrd = cache.Get(reader, "SentDate");
                    deliveredDateOrd = cache.Get(reader, "DeliveredDate");
                    openedDateOrd = cache.Get(reader, "OpenedDate");
                    clickedDateOrd = cache.Get(reader, "ClickedDate");
                    lastUpdatedOrd = cache.Get(reader, "LastUpdated");
                    createdDateOrd = cache.Get(reader, "CreatedDate");
                }

                list.Add(new CampaignLogResponse
                {
                    Id = reader.GetInt64(idOrd),
                    CampaignId = reader.GetInt32(campaignIdOrd),
                    ContactId = reader.GetInt32(contactIdOrd),
                    TouchpointId = reader.GetInt32(touchpointIdOrd),
                    TouchCounter = reader.IsDBNull(touchCounterOrd) ? null : reader.GetInt32(touchCounterOrd),
                    TemplateVersionId = reader.IsDBNull(templateVersionIdOrd) ? null : reader.GetInt32(templateVersionIdOrd),
                    SuccessValue = reader.IsDBNull(successValueOrd) ? null : reader.GetBoolean(successValueOrd),
                    ExecutionStatus = reader.IsDBNull(executionStatusOrd) ? null : reader.GetString(executionStatusOrd),
                    ErrorMessage = reader.IsDBNull(errorMessageOrd) ? null : reader.GetString(errorMessageOrd),
                    SentDate = reader.IsDBNull(sentDateOrd) ? null : reader.GetDateTime(sentDateOrd),
                    DeliveredDate = reader.IsDBNull(deliveredDateOrd) ? null : reader.GetDateTime(deliveredDateOrd),
                    OpenedDate = reader.IsDBNull(openedDateOrd) ? null : reader.GetDateTime(openedDateOrd),
                    ClickedDate = reader.IsDBNull(clickedDateOrd) ? null : reader.GetDateTime(clickedDateOrd),
                    LastUpdated = reader.IsDBNull(lastUpdatedOrd) ? null : reader.GetDateTime(lastUpdatedOrd),
                    CreatedDate = reader.GetDateTime(createdDateOrd)
                });
            }
            return list;
        }
    }
}
