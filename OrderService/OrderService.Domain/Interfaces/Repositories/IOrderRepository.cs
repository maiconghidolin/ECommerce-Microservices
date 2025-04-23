using OrderService.Domain.Entities;

namespace OrderService.Domain.Interfaces.Repositories;

public interface IOrderRepository : ICrudRepository<Order>
{

}