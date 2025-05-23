using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Application.Services;

public abstract class Notification<T>
{
    private readonly INotificationRepository _notificationRepository;

    public Notification(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    protected abstract Notification CreateNotificationEntity(T data);

    protected abstract Task Send(T data);

    public async Task Create(T data)
    {
        await Send(data);

        var entity = CreateNotificationEntity(data);

        await _notificationRepository.Create(entity);
    }

}
