using CatalogService.Infrastructure;
using CatalogService.Presentation.Integration.Tests.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.Presentation.Integration.Tests;

public class BaseTests : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient _client;
    protected readonly CustomWebApplicationFactory _factory;
    protected readonly IServiceScope _scope;
    protected readonly EFContext _dbContext;

    public BaseTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;

        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<EFContext>();
    }

}
