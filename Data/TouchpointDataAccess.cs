using System.Data;
using System.Data.SqlClient;
using RetailConnect.API.Models;

namespace RetailConnect.API.Data
{
    public class TouchpointDataAccess
    {
        private readonly string _connectionString;

        public TouchpointDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
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
                return new TouchpointResponse
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Type = reader.GetString(reader.GetOrdinal("Type")),
                    Configuration = reader.IsDBNull(reader.GetOrdinal("Configuration")) ? null : reader.GetString(reader.GetOrdinal("Configuration")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
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

            while (await reader.ReadAsync())
            {
                list.Add(new TouchpointResponse
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Type = reader.GetString(reader.GetOrdinal("Type")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
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
    }
}
