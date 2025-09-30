using CatalogService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace CatalogService.Presentation.Integration.Tests.Factory;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("CatalogService")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove existing EFContext config
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<EFContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            // Replace with Testcontainers PostgreSQL
            services.AddDbContext<EFContext>(options =>
            {
                options.UseNpgsql(
                    _postgresContainer.GetConnectionString(),
                    x => x.MigrationsAssembly("CatalogService.Infrastructure"));
            });

            services.RemoveAll<IAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, AllowAllAuthorizationHandler>();
        });
    }

    public Task InitializeAsync()
    {
        return _postgresContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _postgresContainer.StopAsync();
    }

}