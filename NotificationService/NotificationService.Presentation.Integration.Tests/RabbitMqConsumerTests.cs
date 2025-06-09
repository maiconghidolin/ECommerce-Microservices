using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NotificationService.Domain.Constants;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Events;
using NotificationService.Infrastructure;
using NotificationService.Presentation.Integration.Tests.Factory;

namespace NotificationService.Presentation.Integration.Tests;

public class RabbitMqConsumerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly IAdvancedBus _advancedBus;
    private readonly IServiceProvider _provider;
    protected readonly IMongoCollection<Notification> _notificationCollection;

    public RabbitMqConsumerTests(CustomWebApplicationFactory factory)
    {
        _provider = factory.Services;

        var bus = _provider.GetRequiredService<IBus>();
        _advancedBus = bus.Advanced;

        var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
        _notificationCollection = context.Notifications;
    }

    [Fact]
    public async Task OrderCreatedMessage_ShouldTriggerNotificationHandlers()
    {
        // Arrange
        await _notificationCollection.DeleteManyAsync(FilterDefinition<Notification>.Empty);

        var message = new OrderCreated
        {
            Id = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            UserEmail = "test@example.com",
            UserNumber = "1234567890"
        };

        var exchange = await _advancedBus.ExchangeDeclareAsync(MessagingSettings.OrderExchangeName, ExchangeType.Topic);

        // Act
        await _advancedBus.PublishAsync(exchange, MessagingSettings.OrderCreatedRoutingKey, false, new Message<OrderCreated>(message));

        // Wait for processing
        await Task.Delay(5000);

        // Assert side effects
        var notifications = await _notificationCollection.Find(FilterDefinition<Notification>.Empty).ToListAsync();

        Assert.NotEmpty(notifications);
        Assert.Contains(notifications, n => n.Type == "Email" && n.UserId == Guid.Parse(message.UserId) && n.EmailAdress == message.UserEmail);
        Assert.Contains(notifications, n => n.Type == "SMS" && n.UserId == Guid.Parse(message.UserId) && n.Number == message.UserNumber);
    }
}
