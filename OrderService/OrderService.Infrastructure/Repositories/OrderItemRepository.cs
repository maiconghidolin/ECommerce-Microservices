using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces.Repositories;

namespace OrderService.Infrastructure.Repositories;

public class OrderItemRepository(EFContext _context) : IOrderItemRepository
{
    public async Task<List<OrderItem>> GetByOrder(Guid orderId)
    {
        return await _context.OrderItems
                        .Where(x => x.OrderId == orderId && x.IsDeleted == false)
                        .AsNoTracking()
                        .ToListAsync();
    }

    public async Task<OrderItem> Get(Guid orderId, Guid id)
    {
        return await _context.OrderItems
                       .Where(x => x.OrderId == orderId && x.Id == id)
                       .AsNoTracking()
                       .FirstOrDefaultAsync();
    }

    public async Task Create(OrderItem orderItem)
    {
        await _context.OrderItems.AddAsync(orderItem);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        await _context.OrderItems
                        .Where(x => x.Id == id)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsDeleted, v => true)
                            .SetProperty(p => p.DeletedAt, v => DateTimeOffset.UtcNow));
    }

}
