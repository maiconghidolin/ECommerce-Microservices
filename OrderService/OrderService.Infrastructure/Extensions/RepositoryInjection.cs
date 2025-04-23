using Microsoft.Extensions.DependencyInjection;
using OrderService.Domain.Interfaces.Repositories;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.Extensions;

public static class RepositoryInjection
{
    public static void AddRepositoryInjection(this IServiceCollection services)
    {
        services.AddTransient<IOrderRepository, OrderRepository>();
        services.AddTransient<IOrderItemRepository, OrderItemRepository>();
        services.AddTransient<IAddressRepository, AddressRepository>();
        services.AddTransient<IPaymentDataRepository, PaymentDataRepository>();
    }
}