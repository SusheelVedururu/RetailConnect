using System.Data;
using Microsoft.Data.SqlClient;
using RetailConnect.API.Models;
using RetailConnect.API.Data.Helpers;

namespace RetailConnect.API.Data
{
    /// <summary>
    /// Data access layer for Segment operations
    /// Executes stored procedures only - NO inline SQL
    /// Performance optimized with OrdinalCache to eliminate redundant GetOrdinal() calls
    /// </summary>
    public class SegmentDataAccess
    {
        private readonly string _connectionString;
        private readonly ILogger<SegmentDataAccess> _logger;

        public SegmentDataAccess(IConfiguration configuration, ILogger<SegmentDataAccess> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _logger = logger;
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
                var cache = new OrdinalCache();
                var idOrd = cache.Get(reader, "Id");
                var nameOrd = cache.Get(reader, "Name");
                var descOrd = cache.Get(reader, "Description");
                var criteriaOrd = cache.Get(reader, "Criteria");
                var isActiveOrd = cache.Get(reader, "IsActive");
                var createdOrd = cache.Get(reader, "CreatedDate");
                var modifiedOrd = cache.Get(reader, "ModifiedDate");

                return new SegmentResponse
                {
                    Id = reader.GetInt32(idOrd),
                    Name = reader.GetString(nameOrd),
                    Description = reader.IsDBNull(descOrd) ? null : reader.GetString(descOrd),
                    Criteria = reader.IsDBNull(criteriaOrd) ? null : reader.GetString(criteriaOrd),
                    IsActive = reader.GetBoolean(isActiveOrd),
                    CreatedDate = reader.GetDateTime(createdOrd),
                    ModifiedDate = reader.IsDBNull(modifiedOrd) ? null : reader.GetDateTime(modifiedOrd)
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

            var cache = new OrdinalCache();
            var idOrd = -1;
            var nameOrd = -1;
            var isActiveOrd = -1;
            var memberCountOrd = -1;

            while (await reader.ReadAsync())
            {
                // Cache ordinals on first read
                if (idOrd == -1)
                {
                    idOrd = cache.Get(reader, "Id");
                    nameOrd = cache.Get(reader, "Name");
                    isActiveOrd = cache.Get(reader, "IsActive");
                    memberCountOrd = cache.Get(reader, "MemberCount");
                }

                segments.Add(new SegmentListItem
                {
                    Id = reader.GetInt32(idOrd),
                    Name = reader.GetString(nameOrd),
                    IsActive = reader.GetBoolean(isActiveOrd),
                    MemberCount = reader.GetInt32(memberCountOrd)
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

        public async Task<bool> DeleteSegmentAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("DELETE FROM RetailConnect.T_Segments WHERE SegmentID = @Id", connection);
                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@Id", id);

                await connection.OpenAsync();
                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting segment {SegmentId}", id);
                throw;
            }
        }

        /// <summary>
        /// Checks if a segment with the given name exists
        /// </summary>
        public async Task<bool> SegmentExistsAsync(string name, int? excludeId = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_CheckSegmentExists", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@ExcludeId", excludeId ?? (object)DBNull.Value);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            var exists = result != null ? Convert.ToInt32(result) : 0;

            return exists > 0;
        }
    }
}
