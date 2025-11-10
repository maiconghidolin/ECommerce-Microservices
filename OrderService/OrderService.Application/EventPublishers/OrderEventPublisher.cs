using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces.EventPublishers;
using OrderService.Application.Models.Events;
using OrderService.Domain.Constants;
using OrderService.Domain.Interfaces.MqPublisher;

namespace OrderService.Application.EventPublishers;

public class OrderEventPublisher : IOrderEventPublisher
{
    private readonly IRabbitMqPublisherManager _rabbitMqPublisherManager;
    private readonly ILogger<OrderEventPublisher> _logger;

    public OrderEventPublisher(IRabbitMqPublisherManager rabbitMqPublisherManager, ILogger<OrderEventPublisher> logger)
    {
        _rabbitMqPublisherManager = rabbitMqPublisherManager;
        _logger = logger;
    }

    public async Task PublishOrderCreatedEvent(Domain.Entities.Order order)
    {
        try
        {
            OrderCreated orderCreated = new()
            {
                Id = order.Id.ToString(),
                UserId = order.UserId.ToString(),
                UserEmail = "fixedemailfortest@mail.com",
                UserNumber = "123456"
            };

            await _rabbitMqPublisherManager.Publish(
                exchangeName: MessagingSettings.OrderExchangeName,
                routingKey: MessagingSettings.OrderCreatedRoutingKey,
                orderCreated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish OrderCreated event for Order {OrderId}", order.Id);
        }
    }

}
