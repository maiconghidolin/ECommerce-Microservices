using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RabbitMQ.Client;

namespace CatalogService.Presentation.Extensions;

public static class ServiceCollectionExtension
{
    public static void Configure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter(); // Exposes /metrics for Prometheus
            })
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CatalogService"))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddNpgsql()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(configuration["Tempo:URI"]);
                    });
            });

        services.AddControllers();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog API", Version = "v1" });

            var basePath = configuration["ApiPathBase"]?.Trim().TrimStart('/');

            if (!string.IsNullOrWhiteSpace(basePath))
                c.AddServer(new OpenApiServer { Url = '/' + basePath });
        });

        services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(configuration["MessagingSettings:URI"]),
                AutomaticRecoveryEnabled = true
            };

            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        services.AddHealthChecks()
            .AddRabbitMQ(
                name: "RabbitMQ",
                failureStatus: HealthStatus.Unhealthy)
            .AddNpgSql(
                connectionString: configuration["Database:ConnectionString"],
                healthQuery: "SELECT 1;",
                name: "Database",
                failureStatus: HealthStatus.Unhealthy);

        services.AddMvc(options =>
        {
            var noContentFormatter = options.OutputFormatters.OfType<HttpNoContentOutputFormatter>().FirstOrDefault();
            if (noContentFormatter != null)
            {
                noContentFormatter.TreatNullValueAsNoContent = false;
            }
        });

        services.AddHttpClient();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"))
            .AddPolicy("OrderManagerOnly", policy => policy.RequireRole("order-manager"))
            .AddPolicy("AdminOrOrderManager", policy => policy.RequireRole("admin", "order-manager"));
    }

}
