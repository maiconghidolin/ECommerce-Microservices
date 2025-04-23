using OrderService.Domain.Entities;

namespace OrderService.Domain.Interfaces.Repositories;

public interface IOrderItemRepository
{
    Task<List<OrderItem>> GetByOrder(Guid orderId);

    Task<OrderItem> Get(Guid orderId, Guid id);

    Task Create(OrderItem orderItem);

    Task Delete(Guid id);
}
