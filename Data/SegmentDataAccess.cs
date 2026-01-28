using System.Data;
using System.Data.SqlClient;
using RetailConnect.API.Models;

namespace RetailConnect.API.Data
{
    /// <summary>
    /// Data access layer for Segment operations
    /// Executes stored procedures only - NO inline SQL
    /// </summary>
    public class SegmentDataAccess
    {
        private readonly string _connectionString;

        public SegmentDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        /// <summary>
        /// Creates a new segment by calling usp_CreateSegment
        /// </summary>
        public async Task<int> CreateSegmentAsync(CreateSegmentRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_CreateSegment", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@Description", request.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Criteria", request.Criteria ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", request.IsActive);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            var segmentId = result != null ? Convert.ToInt32(result) : 0;

            return segmentId;
        }

        /// <summary>
        /// Gets a segment by ID by calling usp_GetSegmentById
        /// </summary>
        public async Task<SegmentResponse?> GetSegmentByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetSegmentById", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@SegmentId", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new SegmentResponse
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) 
                        ? null 
                        : reader.GetString(reader.GetOrdinal("Description")),
                    Criteria = reader.IsDBNull(reader.GetOrdinal("Criteria")) 
                        ? null 
                        : reader.GetString(reader.GetOrdinal("Criteria")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    ModifiedDate = reader.IsDBNull(reader.GetOrdinal("ModifiedDate")) 
                        ? null 
                        : reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
                };
            }

            return null;
        }

        /// <summary>
        /// Gets all segments by calling usp_GetAllSegments
        /// </summary>
        public async Task<List<SegmentListItem>> GetAllSegmentsAsync()
        {
            var segments = new List<SegmentListItem>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetAllSegments", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                segments.Add(new SegmentListItem
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    MemberCount = reader.GetInt32(reader.GetOrdinal("MemberCount"))
                });
            }

            return segments;
        }

        /// <summary>
        /// Updates a segment by calling usp_UpdateSegment
        /// </summary>
        public async Task<bool> UpdateSegmentAsync(int id, UpdateSegmentRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_UpdateSegment", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@SegmentId", id);
            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@Description", request.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Criteria", request.Criteria ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", request.IsActive);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        /// <summary>
        /// Checks if a segment with the given name exists
        /// </summary>
        public async Task<bool> SegmentExistsAsync(string name)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_CheckSegmentExists", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Name", name);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            var exists = result != null ? Convert.ToInt32(result) : 0;

            return exists > 0;
        }
    }
}
