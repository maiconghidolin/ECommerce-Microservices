using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace NotificationService.Infrastructure.Extensions;

public static class DatabaseExtension
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            return new MongoClient(configuration["MongoDB:ConnectionString"]);
        });

        services.AddScoped<MongoDbContext>();
    }
}
