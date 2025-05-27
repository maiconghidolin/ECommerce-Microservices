using Microsoft.Extensions.Configuration;
using Moq;
using OrderService.Application.Interfaces.EventPublishers;
using OrderService.Application.Models;
using OrderService.Application.Unit.Tests.Fixtures;
using OrderService.Domain.Interfaces.Repositories;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace OrderService.Application.Unit.Tests;

public class OrderServiceTests : IClassFixture<WireMockFixture>
{
    private readonly Mock<IOrderRepository> _orderRepository;
    private readonly Mock<IPaymentDataRepository> _paymentDataRepository;
    private readonly Mock<IOrderEventPublisher> _orderEventPublisher;
    private readonly Services.OrderService _orderService;
    private readonly Mock<IHttpClientFactory> _httpClientFactory;
    private readonly WireMockServer _server;

    public OrderServiceTests(WireMockFixture fixture)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "CatalogService:ClientName", "CatalogService" }
            })
            .Build();

        _httpClientFactory = new Mock<IHttpClientFactory>();

        _orderRepository = new Mock<IOrderRepository>();
        _paymentDataRepository = new Mock<IPaymentDataRepository>();
        _orderEventPublisher = new Mock<IOrderEventPublisher>();
        _orderService = new Services.OrderService(_orderRepository.Object, _paymentDataRepository.Object, _orderEventPublisher.Object, _httpClientFactory.Object, config);

        _server = fixture.Server;
    }

    [Fact]
    public async Task GetAll_Should_Return_ListOfOrders()
    {
        // Arrange
        var orders = new List<Domain.Entities.Order>
        {
            new() { Id = Guid.NewGuid(), Status = Domain.Enums.OrderStatus.Pending, UserId = Guid.NewGuid(), CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = Guid.NewGuid(), Status = Domain.Enums.OrderStatus.Pending, UserId = Guid.NewGuid(), CreatedAt = DateTimeOffset.UtcNow }
        };

        _orderRepository.Setup(repo => repo.GetAll()).ReturnsAsync(orders);

        // Act
        var result = await _orderService.GetAll();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(orders.Count, result.Count);
    }

    [Fact]
    public async Task Get_Should_Return_Null_When_NotExists()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        _orderRepository.Setup(repo => repo.Get(id)).ReturnsAsync((Domain.Entities.Order)null);

        // Act
        var result = await _orderService.Get(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Get_Should_Return_Order()
    {
        // Arrange
        var order = new Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            Status = Domain.Enums.OrderStatus.Pending,
            UserId = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        _orderRepository.Setup(repo => repo.Get(order.Id)).ReturnsAsync(order);

        // Act
        var result = await _orderService.Get(order.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
    }

    [Fact]
    public async Task Create_Should_Call_Repository()
    {
        // Arrange
        var order = new Order()
        {
            UserId = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        await _orderService.Create(order);

        // Assert
        _orderRepository.Verify(repo => repo.Create(It.IsAny<Domain.Entities.Order>()), Times.Once);
        _orderEventPublisher.Verify(publisher => publisher.PublishOrderCreatedEvent(It.IsAny<Domain.Entities.Order>()), Times.Once);
    }

    [Fact]
    public async Task SetShippingAddress_Should_Throw_Exception_When_NotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        _orderRepository.Setup(repo => repo.Get(orderId)).ReturnsAsync((Domain.Entities.Order)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _orderService.SetShippingAddress(orderId, addressId));
    }

    [Fact]
    public async Task SetShippingAddress_Should_Call_Repository()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        var order = new Domain.Entities.Order
        {
            Id = orderId,
            Status = Domain.Enums.OrderStatus.Pending,
            UserId = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        _orderRepository.Setup(repo => repo.Get(orderId)).ReturnsAsync(order);

        // Act
        await _orderService.SetShippingAddress(orderId, addressId);

        // Assert
        _orderRepository.Verify(repo => repo.Update(It.IsAny<Domain.Entities.Order>()), Times.Once);
    }

    [Fact]
    public async Task SetPaymentData_Should_Throw_Exception_When_NotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var paymentData = new PaymentData();

        _orderRepository.Setup(repo => repo.Get(orderId)).ReturnsAsync((Domain.Entities.Order)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _orderService.SetPaymentData(orderId, paymentData));
    }

    [Fact]
    public async Task SetPaymentData_Should_Call_Repository()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var paymentData = new PaymentData();

        var order = new Domain.Entities.Order
        {
            Id = orderId,
            Status = Domain.Enums.OrderStatus.Pending,
            UserId = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        _orderRepository.Setup(repo => repo.Get(orderId)).ReturnsAsync(order);

        // Act
        await _orderService.SetPaymentData(orderId, paymentData);

        // Assert
        _paymentDataRepository.Verify(repo => repo.Create(It.IsAny<Domain.Entities.PaymentData>()), Times.Once);
        _orderRepository.Verify(repo => repo.Update(It.IsAny<Domain.Entities.Order>()), Times.Once);
    }

    [Fact]
    public async Task Update_Should_Throw_Exception_When_NotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var orderDTO = new Order();

        _orderRepository.Setup(repo => repo.Get(orderId)).ReturnsAsync((Domain.Entities.Order)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _orderService.Update(orderId, orderDTO));
    }

    [Fact]
    public async Task Update_Should_Call_Repository()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var orderDTO = new Order();

        var order = new Domain.Entities.Order
        {
            Id = orderId,
            Status = Domain.Enums.OrderStatus.Pending,
            UserId = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        _orderRepository.Setup(repo => repo.Get(orderId)).ReturnsAsync(order);

        // Act
        await _orderService.Update(orderId, orderDTO);

        // Assert
        _orderRepository.Verify(repo => repo.Update(It.IsAny<Domain.Entities.Order>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_Call_Repository()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act
        await _orderService.Delete(orderId);

        // Assert
        _orderRepository.Verify(repo => repo.Delete(orderId), Times.Once);
    }

    [Fact]
    public async Task TracingTest_Should_ThrowException()
    {
        // Arrange
        _server.Reset();

        _server
            .Given(Request.Create().WithPath("/products").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Created));

        var httpClient = _server.CreateClient();

        _httpClientFactory
            .Setup(f => f.CreateClient("CatalogService"))
            .Returns(httpClient);

        // Act and Assert
        await Assert.ThrowsAsync<Exception>(() => _orderService.TracingTest(true));

        var postRequests = _server.LogEntries.Where(e =>
            e.RequestMessage.Method == "POST" &&
            e.RequestMessage.Path == "/products"
        ).ToList();

        Assert.Single(postRequests);
    }

    [Fact]
    public async Task TracingTest_Should_CallCatalogService()
    {
        // Arrange
        _server.Reset();

        _server
            .Given(Request.Create().WithPath("/products").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Created));

        _server
            .Given(Request.Create().WithPath("/products/*").UsingDelete())
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.NoContent));

        var httpClient = _server.CreateClient();

        _httpClientFactory
            .Setup(f => f.CreateClient("CatalogService"))
            .Returns(httpClient);

        // Act
        await _orderService.TracingTest(false);

        // Assert
        var postRequests = _server.LogEntries.Where(e =>
            e.RequestMessage.Method == "POST" &&
            e.RequestMessage.Path == "/products"
        ).ToList();

        var deleteRequests = _server.LogEntries.Where(e =>
            e.RequestMessage.Method == "DELETE" &&
            e.RequestMessage.Path.StartsWith("/products/")
        ).ToList();

        Assert.Single(postRequests);
        Assert.Single(deleteRequests);
    }

}
