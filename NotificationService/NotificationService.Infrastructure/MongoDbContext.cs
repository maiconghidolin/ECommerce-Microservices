using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient client, IConfiguration config)
    {
        _database = client.GetDatabase(config["MongoDB:Database"]);
    }

    public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("Notifications");
}
