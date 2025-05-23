using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Constants;
using NotificationService.Domain.Events;
using NotificationService.Domain.Interfaces.MqConsumer;

namespace NotificationService.Infrastructure.MqConsumer;

public class RabbitMqConsumerStartupService : IHostedService
{
    private readonly IRabbitMqConsumerManager _consumerManager;
    private readonly ILogger<RabbitMqConsumerStartupService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMqConsumerStartupService(
        IRabbitMqConsumerManager consumerManager,
        IServiceProvider serviceProvider,
        ILogger<RabbitMqConsumerStartupService> logger)
    {
        _consumerManager = consumerManager;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _consumerManager.TryToConnect();

        _consumerManager.Consume<OrderCreated>(
            exchangeName: MessagingSettings.OrderExchangeName,
            queueName: MessagingSettings.NotificationOrderCreatedQueueName,
            routingKey: MessagingSettings.OrderCreatedRoutingKey,
            async (message, props, info) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler>();
                await handler.SendOrderCreatedNotification(message, props, info);
            }
        );
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}