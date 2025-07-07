using CatalogService.Presentation.Middleware;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

namespace CatalogService.Presentation.Extensions;

public static class WebApplicationExtension
{
    public static void Configure(this WebApplication app, IConfiguration configuration)
    {
        var basePath = configuration["ApiPathBase"]?.Trim().TrimStart('/');

        if (!string.IsNullOrWhiteSpace(basePath))
            app.UsePathBase('/' + basePath);

        app.UseHealthChecks("/health", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseExceptionMiddleware();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseSerilogRequestLogging();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Use((context, next) =>
        {
            context.Request.EnableBuffering();
            return next();
        });

        app.MapPrometheusScrapingEndpoint();
    }
}
