using System.Net;
using System.Text.Json;
using RetailConnect.API.Models;

namespace RetailConnect.API.Middleware
{
    /// <summary>
    /// Global exception handler middleware to catch all unhandled exceptions
    /// and return standardized error responses without leaking internal details
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var correlationId = context.TraceIdentifier;
            
            // Log the full exception details with correlation ID
            _logger.LogError(exception, 
                "Unhandled exception occurred. CorrelationId: {CorrelationId}, Path: {Path}", 
                correlationId, 
                context.Request.Path);

            // Determine status code and user-friendly message based on exception type
            var (statusCode, message) = exception switch
            {
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid request data provided"),
                KeyNotFoundException => (HttpStatusCode.NotFound, "The requested resource was not found"),
                InvalidOperationException => (HttpStatusCode.Conflict, "The operation cannot be completed due to a conflict"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "You are not authorized to perform this action"),
                _ => (HttpStatusCode.InternalServerError, "An error occurred while processing your request")
            };

            var errorResponse = new ApiErrorResponse
            {
                StatusCode = (int)statusCode,
                Message = message,
                CorrelationId = correlationId,
                Path = context.Request.Path,
                Timestamp = DateTime.UtcNow
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, jsonOptions));
        }
    }
}
