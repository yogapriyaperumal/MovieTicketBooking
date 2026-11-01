using System.Net;
using System.Text.Json;

namespace MovieTicketBooking.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An unexpected error occurred. TraceId: {TraceId}",
                context.TraceIdentifier);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();

            var (statusCode, message) = exception switch
            {
                ArgumentException =>
                    (StatusCodes.Status400BadRequest, "Invalid request."),

                KeyNotFoundException =>
                    (StatusCodes.Status404NotFound, "Resource not found."),

                UnauthorizedAccessException =>
                    (StatusCodes.Status403Forbidden, "Access denied."),

                _ =>
                    (StatusCodes.Status500InternalServerError,
                     "An unexpected server error occurred.")
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message,
                details = _environment.IsDevelopment()
                    ? exception.Message
                    : null,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}