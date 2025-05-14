using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Models;
using OrderService.Infrastructure;
using OrderService.Presentation.Integration.Tests.Factory;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace OrderService.Presentation.Integration.Tests;

public class OrderControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient _client;
    protected readonly EFContext _dbContext;

    public OrderControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<EFContext>();
    }

    [Fact]
    public async Task TracingTest_Should_Return_OK()
    {
        var content = new StringContent("false", Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/orders/tracing-test", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_Should_Return_EmptyList_When_NoOrders()
    {
        // Arrange
        var allOrders = _dbContext.Orders.ToList();
        _dbContext.Orders.RemoveRange(allOrders);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/orders");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<Order>>();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAll_Should_Return_Order_When_Exists()
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
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/orders");

        // Assert
        response.EnsureSuccessStatusCode();

        var orders = await response.Content.ReadFromJsonAsync<List<Order>>();
        Assert.NotEmpty(orders);
        Assert.Contains(orders, p => p.Id == order.Id);
    }

    [Fact]
    public async Task Get_Should_Return_NotFound_When_NotExists()
    {
        Guid id = Guid.NewGuid();
        var response = await _client.GetAsync($"/orders/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_Should_Return_Order_When_Exists()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        var order = new Domain.Entities.Order
        {
            Id = id,
            UserId = Guid.NewGuid(),
            Status = Domain.Enums.OrderStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/orders/{id}");

        // Assert
        response.EnsureSuccessStatusCode();

        var orderResonse = await response.Content.ReadFromJsonAsync<Order>();
        Assert.Equal(orderResonse.Id, order.Id);
        Assert.Equal(orderResonse.UserId, order.UserId);
        Assert.Equal(orderResonse.Status, order.Status);
    }

    [Fact]
    public async Task Post_Should_Return_BadRequest_When_InvalidModel()
    {
        // Arrange
        var order = new Order
        {
            UserId = Guid.Empty,
            Status = Domain.Enums.OrderStatus.Pending,
            CreatedAt = DateTimeOffset.MinValue,
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/orders", order);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_Should_Return_Created()
    {
        // Arrange
        var order = new Order
        {
            UserId = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/orders", order);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task SetShippingAddress_Should_Return_BadRequest_When_OrderNotFound()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Guid addressId = Guid.NewGuid();

        // Act
        var content = new StringContent(addressId.ToString(), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"/orders/{id}/set-shipping-address", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SetShippingAddress_Should_Return_Ok()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Guid addressId = Guid.NewGuid();

        var order = new Domain.Entities.Order
        {
            Id = id,
            UserId = userId,
            Status = Domain.Enums.OrderStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };

        _dbContext.Orders.Add(order);

        var address = new Domain.Entities.Address
        {
            Id = addressId,
            UserId = userId,
            Street = "123 Main St",
            Number = "Apt 4B",
            City = "Anytown",
            State = "CA",
            ZipCode = "12345",
        };

        _dbContext.Addresses.Add(address);

        await _dbContext.SaveChangesAsync();

        // Act
        var content = new StringContent($"\"{addressId}\"", Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"/orders/{id}/set-shipping-address", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SetPaymentData_Should_Return_BadRequest_When_OrderNotFound()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        var paymentData = new PaymentData
        {
            PaymentMethod = Domain.Enums.PaymentMethod.CreditCard,
            CardNumber = "123"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/orders/{id}/set-payment-data", paymentData);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SetPaymentData_Should_Return_Ok()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        var order = new Domain.Entities.Order
        {
            Id = id,
            UserId = userId,
            Status = Domain.Enums.OrderStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var paymentData = new PaymentData
        {
            PaymentMethod = Domain.Enums.PaymentMethod.CreditCard,
            CardNumber = "123"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/orders/{id}/set-payment-data", paymentData);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_NoContent()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        var order = new Domain.Entities.Order
        {
            Id = id,
            UserId = Guid.NewGuid(),
            Status = Domain.Enums.OrderStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/orders/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    }

}