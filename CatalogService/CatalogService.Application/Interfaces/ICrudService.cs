using CatalogService.Application.Models;

namespace CatalogService.Application.Interfaces;

public interface ICrudService<T> where T : BaseModel
{
    Task<List<T>> GetAll();

    Task<T> Get(Guid id);

    Task Create(T entity);

    Task Update(Guid id, T entity);

    Task Delete(Guid id);
}