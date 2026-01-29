namespace RetailConnect.API.Models
{
    /// <summary>
    /// Standardized API error response model
    /// </summary>
    public class ApiErrorResponse
    {
        /// <summary>
        /// HTTP status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// User-friendly error message (never exposes internal details)
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Correlation ID for tracing the request through logs
        /// </summary>
        public string? CorrelationId { get; set; }

        /// <summary>
        /// Timestamp when the error occurred
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional path that caused the error
        /// </summary>
        public string? Path { get; set; }
    }
}
