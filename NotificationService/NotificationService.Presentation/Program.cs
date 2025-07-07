using NotificationService.Application.Extensions;
using NotificationService.Infrastructure.Extensions;
using NotificationService.Presentation.Extensions;
using Serilog;

namespace NotificationService.Presentation;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        IConfiguration configuration = builder.Configuration;

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        builder.Host.UseSerilog();

        builder.Services.Configure(configuration);

        builder.Services.ConfigureDatabase(configuration);

        builder.Services.AddServiceInjection();

        builder.Services.AddMessageHandlerInjection();

        builder.Services.AddRepositoryInjection();

        builder.Services.AddMqConsumerInjection();

        var app = builder.Build();

        app.Configure(configuration);

        app.Run();
    }
}
