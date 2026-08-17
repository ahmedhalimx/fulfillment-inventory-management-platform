using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace FulfillmentInventoryPlatform.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var (statusCode, title) = exception switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource Not Found"),
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, "Access Denied"),
            InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid Operation"),
            ArgumentException => (HttpStatusCode.BadRequest, "Bad Request"),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error")
        };

        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        var json = JsonSerializer.Serialize(problemDetails);
        return context.Response.WriteAsync(json);
    }
}
