using OrderService.Application.Models;

namespace OrderService.Application.Interfaces;

public interface IOrderItemService
{
    Task<List<OrderItem>> GetByOrder(Guid orderId);

    Task<OrderItem> Get(Guid orderId, Guid id);

    Task Create(Guid orderId, OrderItem entity);

    Task Delete(Guid orderId, Guid id);
}
