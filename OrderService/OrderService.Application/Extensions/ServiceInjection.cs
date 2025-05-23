using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Extensions;

public static class ServiceInjection
{
    public static void AddServiceInjection(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, Services.OrderService>();
        services.AddScoped<IOrderItemService, Services.OrderItemService>();
        services.AddScoped<IAddressService, Services.AddressService>();
    }
}
