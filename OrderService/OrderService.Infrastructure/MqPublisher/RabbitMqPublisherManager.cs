using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using OrderService.Domain.Interfaces.MqPublisher;

namespace OrderService.Infrastructure.MqPublisher;

public class RabbitMqPublisherManager : IRabbitMqPublisherManager
{
    private readonly IAdvancedBus _advancedBus;
    private readonly ILogger<RabbitMqPublisherManager> _logger;

    public RabbitMqPublisherManager(IBus bus, ILogger<RabbitMqPublisherManager> logger)
    {
        _advancedBus = bus.Advanced;
        _logger = logger;
    }

    public async Task Publish<T>(string exchangeName, string routingKey, T message)
    {
        try
        {
            var exchange = await _advancedBus.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic);

            _logger.LogInformation($"Publishing message to exchange '{exchangeName}' with routing key '{routingKey}': {System.Text.Json.JsonSerializer.Serialize(message)}");

            await _advancedBus.PublishAsync(
                exchange,
                routingKey,
                mandatory: false,
                message: new Message<T>(
                    message
                ));
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Failed to publish message to exchange '{exchangeName}' with routing key '{routingKey}': {System.Text.Json.JsonSerializer.Serialize(message)}");
            throw;
        }
    }

}
