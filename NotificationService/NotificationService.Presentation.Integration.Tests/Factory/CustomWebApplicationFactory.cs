using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Testcontainers.RabbitMq;

namespace NotificationService.Presentation.Integration.Tests.Factory;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private INetwork _network;
    private MongoDbContainer _mongoContainer;
    private RabbitMqContainer _rabbitContainer;
    private string _mongoConnectionString = string.Empty;
    private string _rabbitEasyNetQConnectionString = string.Empty;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove existing EFContext config
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMongoClient));

            if (descriptor != null)
                services.Remove(descriptor);

            descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBus));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddSingleton<IBus>(RabbitHutch.CreateBus(_rabbitEasyNetQConnectionString));

            var testMongoClient = new MongoClient(_mongoConnectionString);

            // Replace with Testcontainers Mongo
            services.AddSingleton<IMongoClient>(testMongoClient);

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

        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:6")
            .WithCleanUp(true)
            .WithUsername("root")
            .WithPassword("password")
            .WithNetwork(_network)
            .WithName($"mongodb-{guid}")
            .WithNetworkAliases($"mongodb-{guid}")
            .Build();

        await _mongoContainer.StartAsync();

        _mongoConnectionString = _mongoContainer.GetConnectionString();
    }

    public new async Task DisposeAsync()
    {
        if (_mongoContainer != null)
        {
            await _mongoContainer.StopAsync();
            await _mongoContainer.DisposeAsync();
        }

        if (_rabbitContainer != null)
        {
            await _rabbitContainer.StopAsync();
            await _rabbitContainer.DisposeAsync();
        }

        if (_network != null)
            await _network.DisposeAsync();
    }

}

