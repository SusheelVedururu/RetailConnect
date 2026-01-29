using System.Diagnostics;

namespace RetailConnect.API.Middleware
{
    /// <summary>
    /// Middleware to monitor and log request performance
    /// Adds X-Response-Time header and warns about slow queries
    /// </summary>
    public class PerformanceMonitoringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMonitoringMiddleware> _logger;
        private readonly int _slowQueryThresholdMs;

        public PerformanceMonitoringMiddleware(
            RequestDelegate next,
            ILogger<PerformanceMonitoringMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _slowQueryThresholdMs = configuration.GetValue<int>("Caching:SlowQueryThresholdMs", 1000);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                // Add response time header
                context.Response.Headers.Append("X-Response-Time", $"{elapsedMs}ms");

                // Log slow queries
                if (elapsedMs > _slowQueryThresholdMs)
                {
                    _logger.LogWarning(
                        "Slow request detected: {Method} {Path} took {ElapsedMs}ms (threshold: {Threshold}ms)",
                        context.Request.Method,
                        context.Request.Path,
                        elapsedMs,
                        _slowQueryThresholdMs);
                }
            }
        }
    }
}
