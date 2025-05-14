using Moq;
using OrderService.Application.Services;
using OrderService.Domain.Interfaces.Repositories;

namespace OrderService.Application.Unit.Tests;

public class OrderItemServiceTests
{
    private readonly Mock<IOrderItemRepository> _orderItemRepository;
    private readonly OrderItemService _orderItemService;

    public OrderItemServiceTests()
    {
        _orderItemRepository = new Mock<IOrderItemRepository>();
        _orderItemService = new OrderItemService(_orderItemRepository.Object);
    }

    [Fact]
    public async Task GetByOrder_Should_ReturnEmpty_WhenNoItems()
    {
        // Arrange
        Guid orderId = Guid.NewGuid();

        _orderItemRepository.Setup(r => r.GetByOrder(orderId)).ReturnsAsync([]);

        // Act
        var result = await _orderItemService.GetByOrder(orderId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByOrder_Should_Return_ListOfOrderItems()
    {
        // Arrange
        Guid orderId = Guid.NewGuid();

        var orderItems = new List<Domain.Entities.OrderItem>
        {
            new() { Id = Guid.NewGuid(), OrderId = orderId, ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 1 },
            new() { Id = Guid.NewGuid(), OrderId = orderId, ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 2 },
        };

        _orderItemRepository.Setup(r => r.GetByOrder(orderId)).ReturnsAsync(orderItems);

        // Act
        var result = await _orderItemService.GetByOrder(orderId);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(orderItems.Count, result.Count);
    }

    [Fact]
    public async Task Get_Should_Return_Null_When_NotExists()
    {
        // Arrange
        Guid orderId = Guid.NewGuid();
        Guid id = Guid.NewGuid();

        _orderItemRepository.Setup(r => r.Get(orderId, id)).ReturnsAsync((Domain.Entities.OrderItem)null);

        // Act
        var result = await _orderItemService.Get(orderId, id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Get_Should_Return_OrderItem()
    {
        // Arrange
        Guid orderId = Guid.NewGuid();
        Guid id = Guid.NewGuid();

        var orderItem = new Domain.Entities.OrderItem
        {
            Id = id,
            OrderId = orderId,
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            UnitPrice = 1,
        };

        _orderItemRepository.Setup(r => r.Get(orderId, id)).ReturnsAsync(orderItem);

        // Act
        var result = await _orderItemService.Get(orderId, id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderItem.Id, result.Id);
    }

    [Fact]
    public async Task Create_Should_CallRepository()
    {
        // Arrange
        Guid orderId = Guid.NewGuid();

        var orderItem = new Models.OrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            UnitPrice = 1,
        };

        // Act
        await _orderItemService.Create(orderId, orderItem);

        // Assert
        _orderItemRepository.Verify(r => r.Create(It.IsAny<Domain.Entities.OrderItem>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_CallRepository()
    {
        // Arrange
        Guid orderId = Guid.NewGuid();
        Guid id = Guid.NewGuid();

        // Act
        await _orderItemService.Delete(orderId, id);

        // Assert
        _orderItemRepository.Verify(r => r.Delete(id), Times.Once);
    }

}
