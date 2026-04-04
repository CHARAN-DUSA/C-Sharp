using System.Net;
using System.Text.Json;

namespace DoctorBooking.API.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex, _env);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        IWebHostEnvironment env)
    {
        context.Response.ContentType = "application/problem+json";

        var (status, title) = exception switch
        {
            UnauthorizedAccessException => (401, "Unauthorized"),
            KeyNotFoundException => (404, "Not Found"),
            InvalidOperationException => (400, "Bad Request"),
            _ => (500, "Internal Server Error")
        };

        context.Response.StatusCode = status;

        // 🔥 SHOW REAL ERROR IN DEVELOPMENT
        var detail = env.IsDevelopment()
            ? exception.Message
            : "An unexpected error occurred.";

        var stack = env.IsDevelopment()
            ? exception.StackTrace
            : null;

        var problem = new
        {
            type = $"https://httpstatuses.io/{status}",
            title = title,
            status = status,
            detail = detail,
            traceId = context.TraceIdentifier,
            stackTrace = stack // only in dev
        };

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
}