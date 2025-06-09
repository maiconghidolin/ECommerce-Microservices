using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Application.Unit.Tests;

public class EmailNotificationTests
{
    private readonly Mock<INotificationRepository> _notificationRepository;
    private readonly Mock<ILogger<Services.EmailNotification>> _logger;
    private readonly Services.EmailNotification _emailNotification;

    public EmailNotificationTests()
    {
        _notificationRepository = new Mock<INotificationRepository>();
        _logger = new Mock<ILogger<Services.EmailNotification>>();

        _emailNotification = new Services.EmailNotification(_notificationRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task Create_Should_CallSend_AndSaveNotificationEntity()
    {
        Models.Email data = new()
        {
            UserId = Guid.NewGuid().ToString(),
            Body = "Test Body",
            EmailAdress = "abc@def.com",
            Subject = "Test Subject"
        };

        Domain.Entities.Notification savedEntity = null;

        _notificationRepository
            .Setup(repo => repo.Create(It.IsAny<Domain.Entities.Notification>()))
            .Callback<Domain.Entities.Notification>(n => savedEntity = n)
            .ReturnsAsync((Domain.Entities.Notification n) => n);

        await _emailNotification.Create(data);

        _notificationRepository.Verify(repo => repo.Create(It.IsAny<Domain.Entities.Notification>()), Times.Once);

        _logger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains("Sending email to") &&
                    v.ToString().Contains(data.EmailAdress) &&
                    v.ToString().Contains(data.Subject)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        Assert.NotNull(savedEntity);
        Assert.Equal("Email", savedEntity.Type);
        Assert.Equal(Guid.Parse(data.UserId), savedEntity.UserId);
        Assert.Equal(data.EmailAdress, savedEntity.EmailAdress);
        Assert.Equal(data.Subject, savedEntity.Subject);
        Assert.Equal(data.Body, savedEntity.Body);
    }
}
