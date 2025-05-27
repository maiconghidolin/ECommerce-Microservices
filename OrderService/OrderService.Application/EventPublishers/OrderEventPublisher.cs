using OrderService.Application.Interfaces.EventPublishers;
using OrderService.Application.Models.Events;
using OrderService.Domain.Constants;
using OrderService.Domain.Interfaces.MqPublisher;

namespace OrderService.Application.EventPublishers;

public class OrderEventPublisher : IOrderEventPublisher
{
    private readonly IRabbitMqPublisherManager _rabbitMqPublisherManager;

    public OrderEventPublisher(IRabbitMqPublisherManager rabbitMqPublisherManager)
    {
        _rabbitMqPublisherManager = rabbitMqPublisherManager;
    }

    public async Task PublishOrderCreatedEvent(Domain.Entities.Order order)
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

}
