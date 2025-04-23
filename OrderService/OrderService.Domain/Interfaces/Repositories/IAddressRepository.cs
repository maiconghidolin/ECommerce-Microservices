using OrderService.Domain.Entities;

namespace OrderService.Domain.Interfaces.Repositories;

public interface IAddressRepository : ICrudRepository<Address>
{
    Task<List<Address>> GetByUser(Guid userId);
}
