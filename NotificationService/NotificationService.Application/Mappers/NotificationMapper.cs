namespace NotificationService.Application.Mappers;

public static class NotificationMapper
{
    public static Domain.Entities.Notification MapToEntity(this Models.Notification notification)
    {
        return new Domain.Entities.Notification
        {
            Id = notification.Id,
            Type = notification.Type,
            UserId = notification.UserId,
            EmailAdress = notification.EmailAdress,
            Number = notification.Number,
            Subject = notification.Subject,
            Body = notification.Body,
            CreatedAt = notification.CreatedAt,
        };
    }

    public static List<Domain.Entities.Notification> MapToEntity(this List<Models.Notification> notifications)
    {
        return notifications.Select(a => a.MapToEntity()).ToList();
    }

    public static Models.Notification MapToModel(this Domain.Entities.Notification notification)
    {
        return new Models.Notification
        {
            Id = notification.Id,
            Type = notification.Type,
            UserId = notification.UserId,
            EmailAdress = notification.EmailAdress,
            Number = notification.Number,
            Subject = notification.Subject,
            Body = notification.Body,
            CreatedAt = notification.CreatedAt,
        };
    }

    public static List<Models.Notification> MapToModel(this List<Domain.Entities.Notification> notifications)
    {
        return notifications.Select(a => a.MapToModel()).ToList();
    }
}
