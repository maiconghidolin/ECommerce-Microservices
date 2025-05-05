using CatalogService.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Repositories;

public class ProductRepository(EFContext _context) : IProductRepository
{
    public async Task<Domain.Entities.Product> Get(Guid id)
    {
        return await _context.Products
                        .FindAsync(id);
    }

    public async Task<List<Domain.Entities.Product>> GetAll()
    {
        return await _context.Products
                        .Where(x => x.IsDeleted == false)
                        .AsNoTracking()
                        .ToListAsync();
    }

    public async Task Create(Domain.Entities.Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Domain.Entities.Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        await _context.Products
                        .Where(x => x.Id == id)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsDeleted, v => true)
                            .SetProperty(p => p.DeletedAt, v => DateTimeOffset.UtcNow));
    }

}
