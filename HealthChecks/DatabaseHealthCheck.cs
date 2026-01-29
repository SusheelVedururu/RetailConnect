using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Data.SqlClient;

namespace RetailConnect.API.HealthChecks
{
    /// <summary>
    /// Health check to verify database connectivity
    /// </summary>
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseHealthCheck> _logger;

        public DatabaseHealthCheck(IConfiguration configuration, ILogger<DatabaseHealthCheck> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    return HealthCheckResult.Unhealthy("Database connection string not configured");
                }

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1";
                command.CommandTimeout = 5; // 5 second timeout for health check

                var result = await command.ExecuteScalarAsync(cancellationToken);

                return HealthCheckResult.Healthy("Database connection successful");
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return HealthCheckResult.Unhealthy("Database connection failed", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during database health check");
                return HealthCheckResult.Unhealthy("Health check error", ex);
            }
        }
    }
}
