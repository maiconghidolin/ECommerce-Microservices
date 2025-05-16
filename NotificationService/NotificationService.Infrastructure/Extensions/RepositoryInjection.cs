using Microsoft.Extensions.DependencyInjection;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Infrastructure.Repositories;

namespace NotificationService.Infrastructure.Extensions;

public static class RepositoryInjection
{
    public static void AddRepositoryInjection(this IServiceCollection services)
    {
        services.AddTransient<INotificationRepository, NotificationRepository>();
    }
}
