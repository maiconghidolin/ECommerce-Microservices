using Microsoft.Extensions.Logging;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Application.Services;

public class EmailNotification : AbstractNotification<Models.Email>
{
    private readonly ILogger<EmailNotification> _logger;

    public EmailNotification(INotificationRepository notificationRepository, ILogger<EmailNotification> logger) : base(notificationRepository)
    {
        _logger = logger;
    }

    protected override Domain.Entities.Notification CreateNotificationEntity(Models.Email data)
    {
        return new Domain.Entities.Notification()
        {
            Type = "Email",
            UserId = Guid.Parse(data.UserId),
            EmailAdress = data.EmailAdress,
            Subject = data.Subject,
            Body = data.Body
        };
    }

    protected override async Task Send(Models.Email data)
    {
        _logger.LogInformation($"Sending email to {data.EmailAdress} with subject {data.Subject}");

        // Implement the logic to send an email notification
        await Task.CompletedTask;
    }
}
