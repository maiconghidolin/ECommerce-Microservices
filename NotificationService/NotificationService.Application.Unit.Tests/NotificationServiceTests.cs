using Moq;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Application.Unit.Tests;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _notificationRepository;
    private readonly Services.NotificationService _notificationService;

    public NotificationServiceTests()
    {
        _notificationRepository = new Mock<INotificationRepository>();

        _notificationService = new Services.NotificationService(_notificationRepository.Object);
    }

    [Fact]
    public async Task GetAll_Should_Return_ListOfNotifications()
    {
        // Arrange
        var notifications = new List<Notification>
        {
            new() {Id = Guid.NewGuid(), Type = "Email", UserId = Guid.NewGuid(), EmailAdress = "test", Body = "Test Body", Subject = "Test Subject"},
            new() {Id = Guid.NewGuid(), Type = "SMS", UserId = Guid.NewGuid(), Number = "1234", Body = "Test Body 2", Subject = "Test Subject 2" }
        };

        _notificationRepository.Setup(repo => repo.GetAll(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(notifications);

        // Act
        var result = await _notificationService.GetAll(0, 10);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(notifications.Count, result.Count);
    }

}
