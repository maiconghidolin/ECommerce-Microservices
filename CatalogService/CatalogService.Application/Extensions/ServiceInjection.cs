using CatalogService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.Application.Extensions;

public static class ServiceInjection
{
    public static void AddServiceInjection(this IServiceCollection services)
    {
        services.AddScoped<IProductService, Services.ProductService>();
    }
}
