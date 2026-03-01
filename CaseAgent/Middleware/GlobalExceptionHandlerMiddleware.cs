using System.Net;
using System.Text.Json;
using CaseAgent.Model.Responses;

namespace CaseAgent.Middleware;

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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Message = "An error occurred while processing your request",
            Errors = new List<string> { exception.Message },
            Timestamp = DateTime.UtcNow
        };

        context.Response.StatusCode = exception switch
        {
            FileNotFoundException => (int)HttpStatusCode.InternalServerError,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var json = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(json);
    }
}
