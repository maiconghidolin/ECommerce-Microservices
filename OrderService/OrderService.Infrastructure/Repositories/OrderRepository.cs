using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Interfaces.Repositories;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository(EFContext _context) : IOrderRepository
{

    public async Task<Domain.Entities.Order> Get(Guid id)
    {
        return await _context.Orders
                        .FindAsync(id);
    }

    public async Task<List<Domain.Entities.Order>> GetAll()
    {
        // need to do the pagination 

        return await _context.Orders
                        .Where(x => x.IsDeleted == false)
                        .AsNoTracking()
                        .ToListAsync();
    }

    public async Task Create(Domain.Entities.Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Domain.Entities.Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        await _context.Orders
                        .Where(x => x.Id == id)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsDeleted, v => true)
                            .SetProperty(p => p.DeletedAt, v => DateTimeOffset.UtcNow));
    }

}
