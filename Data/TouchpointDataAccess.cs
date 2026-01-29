using System.Data;
using Microsoft.Data.SqlClient;
using RetailConnect.API.Models;
using RetailConnect.API.Data.Helpers;

namespace RetailConnect.API.Data
{
    public class TouchpointDataAccess
    {
        private readonly string _connectionString;
        private readonly ILogger<TouchpointDataAccess> _logger;

        public TouchpointDataAccess(IConfiguration configuration, ILogger<TouchpointDataAccess> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _logger = logger;
        }

        public async Task<int> CreateTouchpointAsync(CreateTouchpointRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_CreateTouchpoint", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@Type", request.Type);
            command.Parameters.AddWithValue("@Configuration", request.Configuration ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", request.IsActive);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<TouchpointResponse?> GetTouchpointByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetTouchpointById", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@TouchpointId", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var cache = new OrdinalCache();
                var idOrd = cache.Get(reader, "Id");
                var nameOrd = cache.Get(reader, "Name");
                var typeOrd = cache.Get(reader, "Type");
                var configOrd = cache.Get(reader, "Configuration");
                var isActiveOrd = cache.Get(reader, "IsActive");
                var createdOrd = cache.Get(reader, "CreatedDate");

                return new TouchpointResponse
                {
                    Id = reader.GetInt32(idOrd),
                    Name = reader.GetString(nameOrd),
                    Type = reader.GetString(typeOrd),
                    Configuration = reader.IsDBNull(configOrd) ? null : reader.GetString(configOrd),
                    IsActive = reader.GetBoolean(isActiveOrd),
                    CreatedDate = reader.GetDateTime(createdOrd)
                };
            }
            return null;
        }

        public async Task<List<TouchpointResponse>> GetAllTouchpointsAsync()
        {
            var list = new List<TouchpointResponse>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetAllTouchpoints", connection);
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

                list.Add(new TouchpointResponse
                {
                    Id = reader.GetInt32(idOrd),
                    Name = reader.GetString(nameOrd),
                    Type = reader.GetString(typeOrd),
                    IsActive = reader.GetBoolean(isActiveOrd)
                });
            }
            return list;
        }

        public async Task<bool> UpdateTouchpointAsync(int id, UpdateTouchpointRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_UpdateTouchpoint", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@TouchpointId", id);
            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@Configuration", request.Configuration ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", request.IsActive);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteTouchpointAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("DELETE FROM RetailConnect.T_Touchpoints WHERE TouchpointID = @Id", connection);
            command.CommandType = CommandType.Text;

            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}
