using NotificationService.Application.Interfaces;
using NotificationService.Application.Mappers;
using NotificationService.Application.Models;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Application.Services;

public class NotificationService(INotificationRepository _notificationRepository) : INotificationService
{
    public async Task<List<Notification>> GetAll(int offset, int fetch)
    {
        var notifications = await _notificationRepository.GetAll(offset, fetch);

        var mappedNotifications = notifications.MapToModel();

        return mappedNotifications;
    }
}
