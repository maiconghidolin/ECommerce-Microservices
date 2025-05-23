using Microsoft.Extensions.DependencyInjection;
using OrderService.Domain.Interfaces.Repositories;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.Extensions;

public static class RepositoryInjection
{
    public static void AddRepositoryInjection(this IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IPaymentDataRepository, PaymentDataRepository>();
    }
}