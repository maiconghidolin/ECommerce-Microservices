using OrderService.Domain.Entities;

namespace OrderService.Domain.Interfaces.Repositories;

public interface ICrudRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAll();

    Task<T> Get(Guid id);

    Task Create(T entity);

    Task Update(T entity);

    Task Delete(Guid id);
}

