using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.EventPublishers;
using OrderService.Application.Interfaces;
using OrderService.Application.Interfaces.EventPublishers;

namespace OrderService.Application.Extensions;

public static class ApplicationInjection
{
    public static void AddServiceInjection(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, Services.OrderService>();
        services.AddScoped<IOrderItemService, Services.OrderItemService>();
        services.AddScoped<IAddressService, Services.AddressService>();
    }

    public static void AddEventPublishersInjection(this IServiceCollection services)
    {
        services.AddScoped<IOrderEventPublisher, OrderEventPublisher>();
    }
}
