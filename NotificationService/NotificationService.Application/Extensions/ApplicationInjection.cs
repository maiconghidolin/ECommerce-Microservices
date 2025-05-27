using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Interfaces;
using NotificationService.Application.MqConsumer;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces.MqConsumer;

namespace NotificationService.Application.Extensions;

public static class ApplicationInjection
{
    public static void AddServiceInjection(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, Services.NotificationService>();

        services.AddScoped<AbstractNotification<Models.Email>, EmailNotification>();
        services.AddScoped<AbstractNotification<Models.SMS>, SmsNotification>();
    }

    public static void AddMessageHandlerInjection(this IServiceCollection services)
    {
        services.AddScoped<IMessageHandler, MessageHandler>();
    }
}
