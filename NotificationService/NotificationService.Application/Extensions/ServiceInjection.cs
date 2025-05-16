using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Application.Extensions;

public static class ServiceInjection
{
    public static void AddServiceInjection(this IServiceCollection services)
    {
        //services.AddTransient<IOrderService, Services.OrderService>();
        //services.AddTransient<IOrderItemService, Services.OrderItemService>();
        //services.AddTransient<IAddressService, Services.AddressService>();
    }
}
