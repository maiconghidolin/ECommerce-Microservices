using MongoDB.Driver;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<Notification> _collection;

    public NotificationRepository(MongoDbContext context)
    {
        _collection = context.Notifications;
    }

    public async Task<List<Notification>> GetAll(int offset, int fetch)
    {
        var filter = Builders<Notification>.Filter.Empty;

        return await _collection
            .Find(filter)
            .Skip(offset)
            .Limit(fetch)
            .ToListAsync();
    }

    public async Task<Notification> Get(Guid id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Notification> Create(Notification notification)
    {
        notification.Id = Guid.NewGuid();
        notification.CreatedAt = DateTime.Now;

        await _collection.InsertOneAsync(notification);
        return notification;
    }

    public async Task Delete(Guid id)
    {
        await _collection.DeleteOneAsync(c => c.Id == id);
    }
}
