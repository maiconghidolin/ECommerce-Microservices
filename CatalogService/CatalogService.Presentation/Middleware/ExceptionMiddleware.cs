using System.Net;
using System.Text.Json;

namespace CatalogService.Presentation.Middleware;

public static class ExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>(Array.Empty<object>());
    }
}

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

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

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError; // 500 if unexpected

        _logger.LogError(exception, exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "An unexpected error occurred",
            traceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }
}

