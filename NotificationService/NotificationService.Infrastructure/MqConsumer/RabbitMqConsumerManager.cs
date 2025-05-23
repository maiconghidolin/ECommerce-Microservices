using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Interfaces.MqConsumer;
using System.Text;

namespace NotificationService.Infrastructure.MqConsumer;

public class RabbitMqConsumerManager : IRabbitMqConsumerManager
{
    private readonly IAdvancedBus _advancedBus;
    private readonly ILogger<RabbitMqConsumerManager> _logger;

    public RabbitMqConsumerManager(IBus bus, ILogger<RabbitMqConsumerManager> logger)
    {
        _advancedBus = bus.Advanced;
        _logger = logger;

        _logger.LogDebug($"RabbitMqConsumerManager created: {this.GetHashCode()}");

        _advancedBus.Connected += OnConnected;
        _advancedBus.Disconnected += OnDisconnected;
    }

    private void OnConnected(object sender, ConnectedEventArgs e)
    {
        _logger.LogDebug($"Connected to RabbitMQ {_advancedBus.GetHashCode()}.");
    }

    private void OnDisconnected(object sender, DisconnectedEventArgs e)
    {
        _logger.LogDebug($"Disconnected from RabbitMQ {_advancedBus.GetHashCode()}.");
    }

    public void Consume<T>(string exchangeName, string queueName, string routingKey, Func<T, MessageProperties, MessageReceivedInfo, Task> handler, string exchangeType = ExchangeType.Topic, bool durable = true)
    {
        _logger.LogInformation($"Declaring exchange '{exchangeName}' of type '{exchangeType}' with durability '{durable}'.");
        var exchange = _advancedBus.ExchangeDeclare(exchangeName, exchangeType, durable: durable, autoDelete: false);

        _logger.LogInformation($"Declaring queue '{queueName}' with durability '{durable}'.");
        var queue = _advancedBus.QueueDeclare(queueName, durable: durable, exclusive: false, autoDelete: false);

        _logger.LogInformation($"Binding queue '{queueName}' to exchange '{exchangeName}' with routing key '{routingKey}'.");
        _advancedBus.Bind(exchange, queue, routingKey);

        _advancedBus.Consume(queue, async (ReadOnlyMemory<byte> body, MessageProperties properties, MessageReceivedInfo info) =>
        {
            try
            {
                _logger.LogInformation($"Received message: {Encoding.UTF8.GetString(body.Span)}");

                T arg = System.Text.Json.JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body.Span));

                await handler(arg, properties, info);

                _logger.LogInformation($"Processed message: {Encoding.UTF8.GetString(body.Span)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process message: {Encoding.UTF8.GetString(body.Span)}");
                // send to DLQ
                throw;
            }
        });
    }

    public async Task TryToConnect()
    {
        while (!_advancedBus.IsConnected)
        {
            try
            {
                await _advancedBus.ConnectAsync();
                _logger.LogInformation("Connected to RabbitMQ.");
            }
            catch (Exception)
            {
                _logger.LogError("Failed to connect to RabbitMQ. Trying again...");
                Thread.Sleep(5000);
            }
        }
    }

}