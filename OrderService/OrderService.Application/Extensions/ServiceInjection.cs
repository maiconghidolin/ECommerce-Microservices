using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Extensions;

public static class ServiceInjection
{
    public static void AddServiceInjection(this IServiceCollection services)
    {
        services.AddTransient<IOrderService, Services.OrderService>();
        services.AddTransient<IOrderItemService, Services.OrderItemService>();
        services.AddTransient<IAddressService, Services.AddressService>();
    }
}
