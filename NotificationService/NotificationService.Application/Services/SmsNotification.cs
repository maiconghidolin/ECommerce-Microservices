using Microsoft.Extensions.Logging;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Application.Services;

public class SmsNotification : Notification<Models.SMS>
{
    private readonly ILogger<SmsNotification> _logger;

    public SmsNotification(INotificationRepository notificationRepository, ILogger<SmsNotification> logger) : base(notificationRepository)
    {
        _logger = logger;
    }

    protected override Domain.Entities.Notification CreateNotificationEntity(Models.SMS data)
    {
        return new Domain.Entities.Notification()
        {
            Type = "SMS",
            Number = data.Number,
            Subject = data.Subject,
            Body = data.Body
        };
    }

    protected override async Task Send(Models.SMS data)
    {
        _logger.LogInformation($"Sending SMS to {data.Number} with subject {data.Subject}");

        // Implement the logic to send an sms notification
        await Task.CompletedTask;
    }
}
