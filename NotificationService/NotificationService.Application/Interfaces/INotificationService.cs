using NotificationService.Application.Models;

namespace NotificationService.Application.Interfaces;

public interface INotificationService
{
    Task<List<Notification>> GetAll(int offset, int fetch);
}
