using OrderService.Application.Extensions;
using OrderService.Infrastructure.Extensions;
using OrderService.Presentation.Extensions;
using Serilog;

namespace OrderService.Presentation;

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

        builder.Services.AddRepositoryInjection();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            DatabaseExtension.RunMigrations(scope.ServiceProvider);
        }

        app.Configure();

        app.Run();
    }
}
