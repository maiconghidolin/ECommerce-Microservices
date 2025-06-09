using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Application.MqConsumer;
using NotificationService.Application.Services;
using NotificationService.Domain.Events;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Application.Unit.Tests;

public class MessageHandlerTests
{
    private readonly Mock<AbstractNotification<Models.Email>> _emailNotification;
    private readonly Mock<AbstractNotification<Models.SMS>> _smsNotification;
    private readonly Mock<ILogger<MessageHandler>> _logger;
    private readonly MessageHandler _messageHandler;

    public MessageHandlerTests()
    {
        var repoMock = new Mock<INotificationRepository>();

        _emailNotification = new Mock<AbstractNotification<Models.Email>>(repoMock.Object);
        _smsNotification = new Mock<AbstractNotification<Models.SMS>>(repoMock.Object);
        _logger = new Mock<ILogger<MessageHandler>>();

        _messageHandler = new MessageHandler(_emailNotification.Object, _smsNotification.Object, _logger.Object);
    }

    [Fact]
    public async Task SendOrderCreatedNotification_Should_CreateEmailNotification_WhenEmailIsInformed()
    {
        // Arrange
        var orderCreatedEvent = new OrderCreated()
        {
            Id = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            UserEmail = "mail@mail.com",
            UserNumber = null
        };

        _emailNotification.Setup(e => e.Create(It.IsAny<Models.Email>()))
            .Returns(Task.CompletedTask);

        // Act
        await _messageHandler.SendOrderCreatedNotification(orderCreatedEvent, null, null);

        // Assert
        _logger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Received OrderCreated message")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        _emailNotification.Verify(repo => repo.Create(It.IsAny<Models.Email>()), Times.Once);
    }

    [Fact]
    public async Task SendOrderCreatedNotification_Should_CreateSmsNotification_WhenNumberIsInformed()
    {
        // Arrange
        var orderCreatedEvent = new OrderCreated()
        {
            Id = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            UserEmail = null,
            UserNumber = "12345"
        };

        _smsNotification.Setup(e => e.Create(It.IsAny<Models.SMS>()))
            .Returns(Task.CompletedTask);

        // Act
        await _messageHandler.SendOrderCreatedNotification(orderCreatedEvent, null, null);

        // Assert
        _logger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Received OrderCreated message")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        _smsNotification.Verify(repo => repo.Create(It.IsAny<Models.SMS>()), Times.Once);
    }

    [Fact]
    public async Task SendOrderCreatedNotification_Should_CreateEmailAndSmsNotification_WhenEmailAndNumberAreInformed()
    {
        // Arrange
        var orderCreatedEvent = new OrderCreated()
        {
            Id = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            UserEmail = "mail@mail.com",
            UserNumber = "12345"
        };

        _emailNotification.Setup(e => e.Create(It.IsAny<Models.Email>()))
            .Returns(Task.CompletedTask);

        _smsNotification.Setup(e => e.Create(It.IsAny<Models.SMS>()))
           .Returns(Task.CompletedTask);

        // Act
        await _messageHandler.SendOrderCreatedNotification(orderCreatedEvent, null, null);

        // Assert
        _logger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Received OrderCreated message")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        _emailNotification.Verify(repo => repo.Create(It.IsAny<Models.Email>()), Times.Once);

        _smsNotification.Verify(repo => repo.Create(It.IsAny<Models.SMS>()), Times.Once);
    }

}
