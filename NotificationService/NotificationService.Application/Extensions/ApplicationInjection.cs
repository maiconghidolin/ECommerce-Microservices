using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.MqConsumer;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces.MqConsumer;

namespace NotificationService.Application.Extensions;

public static class ApplicationInjection
{
    public static void AddServiceInjection(this IServiceCollection services)
    {
        services.AddScoped<Notification<Models.Email>, EmailNotification>();
        services.AddScoped<Notification<Models.SMS>, SmsNotification>();
    }

    public static void AddMessageHandlerInjection(this IServiceCollection services)
    {
        services.AddScoped<IMessageHandler, MessageHandler>();
    }
}
