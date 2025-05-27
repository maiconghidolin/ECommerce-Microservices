namespace OrderService.Domain.Interfaces.MqPublisher;

public interface IRabbitMqPublisherManager
{
    Task Publish<T>(string exchangeName, string routingKey, T message);
}