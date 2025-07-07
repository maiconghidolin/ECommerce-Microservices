using EasyNetQ;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace NotificationService.Presentation.Extensions;

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
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("NotificationService"))
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
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(configuration["Tempo:URI"]);
                    });
            });

        services.AddControllers();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Notification API", Version = "v1" });

            var basePath = configuration["ApiPathBase"]?.Trim().TrimStart('/');

            if (!string.IsNullOrWhiteSpace(basePath))
                c.AddServer(new OpenApiServer { Url = '/' + basePath });
        });

        services.AddSingleton<IBus>(RabbitHutch.CreateBus(configuration["MessagingSettings:EasyNetQConnectionString"]));

        services.AddHealthChecks()
            .AddRabbitMQ(
                configuration["MessagingSettings:URI"],
                name: "RabbitMQ",
                failureStatus: HealthStatus.Unhealthy)
            .AddMongoDb();

        services.AddMvc(options =>
        {
            var noContentFormatter = options.OutputFormatters.OfType<HttpNoContentOutputFormatter>().FirstOrDefault();
            if (noContentFormatter != null)
            {
                noContentFormatter.TreatNullValueAsNoContent = false;
            }
        });

        services.AddHttpClient();

    }

}