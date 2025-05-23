using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.Domain.Entities;

public class Notification
{
    [BsonId]
    public Guid Id { get; set; }

    public string Type { get; set; }

    public Guid UserId { get; set; }

    public string EmailAdress { get; set; }

    public string Number { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }

    public DateTime CreatedAt { get; set; }
}
