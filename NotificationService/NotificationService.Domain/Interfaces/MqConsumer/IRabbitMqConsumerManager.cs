using EasyNetQ;
using EasyNetQ.Topology;

namespace NotificationService.Domain.Interfaces.MqConsumer;

public interface IRabbitMqConsumerManager
{
    void Consume<T>(string exchangeName, string queueName, string routingKey, Func<T, MessageProperties, MessageReceivedInfo, Task> handler, string exchangeType = ExchangeType.Topic, bool durable = true);

    Task TryToConnect();
}
