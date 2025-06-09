using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Application.Services;

public abstract class AbstractNotification<T>
{
    private readonly INotificationRepository _notificationRepository;

    public AbstractNotification(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    protected abstract Notification CreateNotificationEntity(T data);

    protected abstract Task Send(T data);

    public virtual async Task Create(T data)
    {
        await Send(data);

        var entity = CreateNotificationEntity(data);

        await _notificationRepository.Create(entity);
    }

}
