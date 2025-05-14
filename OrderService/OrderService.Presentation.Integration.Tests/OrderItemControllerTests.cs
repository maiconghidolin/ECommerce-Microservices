using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Models;
using OrderService.Infrastructure;
using OrderService.Presentation.Integration.Tests.Factory;
using System.Net;
using System.Net.Http.Json;

namespace OrderService.Presentation.Integration.Tests;

public class OrderItemControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient _client;
    protected readonly EFContext _dbContext;

    public OrderItemControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<EFContext>();
    }

    [Fact]
    public async Task GetAll_Should_Return_EmptyList_When_NoOrderItems()
    {
        // Arrange
        var allOrderItems = _dbContext.OrderItems.ToList();
        _dbContext.OrderItems.RemoveRange(allOrderItems);
        await _dbContext.SaveChangesAsync();

        Guid orderId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/orders/{orderId}/itens");

        // Assert
        response.EnsureSuccessStatusCode();

        var orderItems = await response.Content.ReadFromJsonAsync<List<OrderItem>>();

        Assert.Empty(orderItems);
    }

    [Fact]
    public async Task GetAll_Should_Return_OrderItem_When_Exists()
    {
        // Arrange
        var order = new Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Status = Domain.Enums.OrderStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };

        _dbContext.Orders.Add(order);

        var orderItem = new Domain.Entities.OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            ProductId = Guid.NewGuid(),
            Quantity = 2,
            UnitPrice = 19.99m,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.OrderItems.Add(orderItem);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/orders/{orderItem.OrderId}/itens");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<OrderItem>>();
        Assert.NotEmpty(result);
        Assert.Contains(result, p => p.Id == orderItem.Id);
        Assert.Contains(result, p => p.ProductId == orderItem.ProductId);
    }

    [Fact]
    public async Task Get_Should_Return_NotFound_When_NotExists()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Guid orderId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/orders/{orderId}/itens/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_Should_Return_OrderItem_When_Exists()
    {
        Guid id = Guid.NewGuid();
        Guid orderId = Guid.NewGuid();

        // Arrange
        var order = new Domain.Entities.Order
        {
            Id = orderId,
            UserId = Guid.NewGuid(),
            Status = Domain.Enums.OrderStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };

        _dbContext.Orders.Add(order);

        var orderItem = new Domain.Entities.OrderItem
        {
            Id = id,
            OrderId = order.Id,
            ProductId = Guid.NewGuid(),
            Quantity = 2,
            UnitPrice = 19.99m,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.OrderItems.Add(orderItem);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/orders/{orderId}/itens/{id}");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OrderItem>();

        Assert.Equal(result.Id, orderItem.Id);
        Assert.Equal(result.ProductId, orderItem.ProductId);
    }

    [Fact]
    public async Task Post_Should_Return_BadRequest_When_InvalidModel()
    {
        // Arrange
        var orderItem = new OrderItem
        {
            Id = Guid.NewGuid(),
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/orders/{orderItem.OrderId}/itens", orderItem);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_Should_Return_Created()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Guid orderId = Guid.NewGuid();

        // Arrange
        var order = new Domain.Entities.Order
        {
            Id = orderId,
            UserId = Guid.NewGuid(),
            Status = Domain.Enums.OrderStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };

        _dbContext.Orders.Add(order);

        await _dbContext.SaveChangesAsync();

        var orderItem = new OrderItem
        {
            Id = id,
            OrderId = orderId,
            ProductId = Guid.NewGuid(),
            Quantity = 2,
            UnitPrice = 19.99m
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/orders/{orderItem.OrderId}/itens", orderItem);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_NoContent()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Guid orderId = Guid.NewGuid();

        var order = new Domain.Entities.Order
        {
            Id = orderId,
            UserId = Guid.NewGuid(),
            Status = Domain.Enums.OrderStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };

        _dbContext.Orders.Add(order);

        var orderItem = new Domain.Entities.OrderItem
        {
            Id = id,
            OrderId = order.Id,
            ProductId = Guid.NewGuid(),
            Quantity = 2,
            UnitPrice = 19.99m,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.OrderItems.Add(orderItem);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/orders/{orderItem.OrderId}/itens/{orderItem.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

}

