using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<List<Notification>> GetAll(int offset, int fetch);

    Task<Notification> Get(Guid id);

    Task<Notification> Create(Notification notification);

    Task Delete(Guid id);
}
