using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces.Repositories;

namespace OrderService.Infrastructure.Repositories;

public class AddressRepository(EFContext _context) : IAddressRepository
{

    public async Task<Domain.Entities.Address> Get(Guid id)
    {
        return await _context.Addresses
                        .FindAsync(id);
    }

    public async Task<List<Domain.Entities.Address>> GetAll()
    {
        // need to do the pagination 

        return await _context.Addresses
                        .Where(x => x.IsDeleted == false)
                        .AsNoTracking()
                        .ToListAsync();
    }

    public async Task<List<Address>> GetByUser(Guid userId)
    {
        return await _context.Addresses
                        .Where(x => x.IsDeleted == false && x.UserId == userId)
                        .AsNoTracking()
                        .ToListAsync();
    }

    public async Task Create(Domain.Entities.Address address)
    {
        await _context.Addresses.AddAsync(address);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Domain.Entities.Address address)
    {
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        await _context.Addresses
                        .Where(x => x.Id == id)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsDeleted, v => true)
                            .SetProperty(p => p.DeletedAt, v => DateTimeOffset.UtcNow));
    }

}