using Microsoft.Extensions.DependencyInjection;
using OrderService.Domain.Interfaces.MqPublisher;
using OrderService.Domain.Interfaces.Repositories;
using OrderService.Infrastructure.MqPublisher;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.Extensions;

public static class InfrastructureInjection
{
    public static void AddRepositoryInjection(this IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IPaymentDataRepository, PaymentDataRepository>();
    }

    public static void AddMqPublisherInjection(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitMqPublisherManager, RabbitMqPublisherManager>();
    }
}