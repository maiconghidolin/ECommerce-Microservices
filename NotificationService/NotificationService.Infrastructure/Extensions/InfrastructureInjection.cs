using Microsoft.Extensions.DependencyInjection;
using NotificationService.Domain.Interfaces.MqConsumer;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Infrastructure.MqConsumer;
using NotificationService.Infrastructure.Repositories;

namespace NotificationService.Infrastructure.Extensions;

public static class InfrastructureInjection
{
    public static void AddRepositoryInjection(this IServiceCollection services)
    {
        services.AddScoped<INotificationRepository, NotificationRepository>();
    }

    public static void AddMqConsumerInjection(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitMqConsumerManager, RabbitMqConsumerManager>();
        services.AddHostedService<RabbitMqConsumerStartupService>();
    }

}
