using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Interfaces.Repositories;

namespace OrderService.Infrastructure.Repositories;

public class PaymentDataRepository(EFContext _context) : IPaymentDataRepository
{

    public async Task<Domain.Entities.PaymentData> Get(Guid id)
    {
        return await _context.PaymentData
                        .FindAsync(id);
    }

    public async Task<List<Domain.Entities.PaymentData>> GetAll()
    {
        return await _context.PaymentData
                        .Where(x => x.IsDeleted == false)
                        .AsNoTracking()
                        .ToListAsync();
    }

    public async Task Create(Domain.Entities.PaymentData paymentData)
    {
        await _context.PaymentData.AddAsync(paymentData);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Domain.Entities.PaymentData paymentData)
    {
        _context.PaymentData.Update(paymentData);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        await _context.PaymentData
                        .Where(x => x.Id == id)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsDeleted, v => true)
                            .SetProperty(p => p.DeletedAt, v => DateTimeOffset.UtcNow));
    }

}
