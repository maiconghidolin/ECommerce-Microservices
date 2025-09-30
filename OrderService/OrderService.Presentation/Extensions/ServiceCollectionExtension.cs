using EasyNetQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OrderService.Presentation.Extensions;

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
                    .AddPrometheusExporter();
            })
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("OrderService"))
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            activity.DisplayName = $"{httpRequest.Method} {httpRequest.Path}";
                        };

                        options.EnrichWithException = (activity, exception) =>
                        {
                            if (exception.Source != null)
                            {
                                activity.SetTag("exception.source", exception.Source);
                            }
                        };
                    })
                    .AddHttpClientInstrumentation((options) =>
                    {
                        options.EnrichWithHttpRequestMessage = (activity, httpRequestMessage) =>
                        {
                            activity.DisplayName = $"{httpRequestMessage.Method} {httpRequestMessage.RequestUri}";
                        };

                        options.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("stackTrace", exception.StackTrace);
                        };
                    })
                    .AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;

                        options.EnrichWithIDbCommand = (activity, command) =>
                        {
                            var keyword = command.CommandText?.Split(' ', '\n')[0];
                            activity.DisplayName = $"EF: {keyword}";
                            activity.SetTag("db.statement", command.CommandText);
                        };
                    })
                    .AddNpgsql()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(configuration["Tempo:URI"]);
                    });
            });

        services
            .AddControllers()
            .AddNewtonsoftJson();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order API", Version = "v1" });

            var basePath = configuration["ApiPathBase"]?.Trim().TrimStart('/');

            if (!string.IsNullOrWhiteSpace(basePath))
                c.AddServer(new OpenApiServer { Url = '/' + basePath });
        });

        services.AddHttpContextAccessor();

        services.AddSingleton<IBus>(RabbitHutch.CreateBus(configuration["MessagingSettings:EasyNetQConnectionString"]));

        services.AddHealthChecks()
            .AddRabbitMQ(
                configuration["MessagingSettings:URI"],
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

        services.AddHttpClient(configuration["CatalogService:ClientName"], (serviceProvider, client) =>
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

            client.BaseAddress = new Uri(configuration["CatalogService:URI"]);

            var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Add("Authorization", token);

        });

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