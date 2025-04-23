using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace OrderService.Presentation.Extensions;

public static class WebApplicationExtension
{
    public static void Configure(this WebApplication app)
    {
        app.UseHealthChecks("/health", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Use((context, next) =>
        {
            context.Request.EnableBuffering();
            return next();
        });
    }
}
