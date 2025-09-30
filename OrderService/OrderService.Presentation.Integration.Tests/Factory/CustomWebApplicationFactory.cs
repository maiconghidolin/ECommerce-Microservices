using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderService.Infrastructure;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace OrderService.Presentation.Integration.Tests.Factory;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private INetwork _network;
    private PostgreSqlContainer _postgresContainer;
    private RabbitMqContainer _rabbitContainer;
    private IContainer _catalogServiceContainer;
    private string _orderServiceConnectionString = string.Empty;
    private string _catalogServiceUrl;
    private string _rabbitEasyNetQConnectionString = string.Empty;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var configOverrides = new Dictionary<string, string>
            {
                ["CatalogService:URI"] = _catalogServiceUrl,
            };

            configBuilder.AddInMemoryCollection(configOverrides);
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove existing EFContext config
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<EFContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBus));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddSingleton<IBus>(RabbitHutch.CreateBus(_rabbitEasyNetQConnectionString));

            // Replace with Testcontainers PostgreSQL
            services.AddDbContext<EFContext>(options =>
            {
                options.UseNpgsql(
                    _orderServiceConnectionString,
                    x => x.MigrationsAssembly("OrderService.Infrastructure"));
            });

            services.RemoveAll<IAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, AllowAllAuthorizationHandler>();
        });
    }

    public async Task InitializeAsync()
    {
        Guid guid = Guid.NewGuid();

        _network = new NetworkBuilder()
            .WithName($"ecommerce-network-{guid}")
            .Build();

        _rabbitContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:3-management-alpine")
            .WithUsername("guest")
            .WithPassword("guest")
            .WithNetwork(_network)
            .WithName($"rabbitmq-{guid}")
            .WithNetworkAliases($"rabbitmq-{guid}")
            .WithCleanUp(true)
            .WithPortBinding(5672, true)
            .WithPortBinding(15672, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
            .Build();

        await _rabbitContainer.StartAsync();

        _rabbitEasyNetQConnectionString = $"host={_rabbitContainer.Hostname}:{_rabbitContainer.GetMappedPublicPort(5672)};username=guest;password=guest";

        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("OrderService")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithNetwork(_network)
            .WithName($"postgres-{guid}")
            .WithNetworkAliases($"postgres-{guid}")
            .Build();

        await _postgresContainer.StartAsync();

        var originalConnectionString = _postgresContainer.GetConnectionString();
        _orderServiceConnectionString = originalConnectionString;
        var catalogServiceConnectionString = OverrideDatabaseProperties(originalConnectionString, $"postgres-{guid}", "CatalogService");

        _catalogServiceContainer = new ContainerBuilder()
            .WithImage("maiconghidolin/ecommerce-catalog-service:latest")
            .WithNetwork(_network)
            .WithPortBinding(8080, true)
            .WithName($"catalog-service-{guid}")
            .WithNetworkAliases("catalog-service")
            .WithEnvironment("Database__ConnectionString", catalogServiceConnectionString)
            .Build();

        await _catalogServiceContainer.StartAsync();

        var requestUri = new UriBuilder(Uri.UriSchemeHttp, _catalogServiceContainer.Hostname, _catalogServiceContainer.GetMappedPublicPort(8080)).Uri;

        _catalogServiceUrl = requestUri.ToString();
    }

    public new async Task DisposeAsync()
    {
        if (_postgresContainer != null)
        {
            await _postgresContainer.StopAsync();
            await _postgresContainer.DisposeAsync();
        }

        if (_rabbitContainer != null)
        {
            await _rabbitContainer.StopAsync();
            await _rabbitContainer.DisposeAsync();
        }

        if (_catalogServiceContainer != null)
        {
            await _catalogServiceContainer.StopAsync();
            await _catalogServiceContainer.DisposeAsync();
        }

        if (_network != null)
            await _network.DisposeAsync();
    }

    private string OverrideDatabaseProperties(string originalConnStr, string host, string newDbName)
    {
        var builder = new Npgsql.NpgsqlConnectionStringBuilder(originalConnStr)
        {
            Host = host,
            Port = 5432,
            Database = newDbName
        };

        return builder.ToString();
    }

}
