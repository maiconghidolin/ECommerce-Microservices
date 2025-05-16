using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace NotificationService.Infrastructure.Extensions;

public static class DatabaseExtension
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            return new MongoClient(configuration["MongoDB:ConnectionString"]);
        });

        services.AddScoped<MongoDbContext>();
    }
}
