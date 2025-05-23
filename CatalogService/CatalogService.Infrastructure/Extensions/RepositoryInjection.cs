using CatalogService.Domain.Interfaces.Repositories;
using CatalogService.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.Infrastructure.Extensions;

public static class RepositoryInjection
{
    public static void AddRepositoryInjection(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
    }
}
