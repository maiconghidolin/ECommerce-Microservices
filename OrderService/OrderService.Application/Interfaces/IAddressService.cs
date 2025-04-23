using OrderService.Application.Models;

namespace OrderService.Application.Interfaces;

public interface IAddressService : ICrudService<Address>
{
    Task<List<Address>> GetByUser(Guid userId);

}
