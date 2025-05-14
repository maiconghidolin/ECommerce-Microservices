using OrderService.Application.Interfaces;
using OrderService.Application.Mappers;
using OrderService.Application.Models;
using OrderService.Domain.Interfaces.Repositories;

namespace OrderService.Application.Services;

public class AddressService(IAddressRepository _addressRepository) : IAddressService
{

    public async Task<List<Address>> GetAll()
    {
        var addresses = await _addressRepository.GetAll();

        var mappedAddresses = addresses.MapToModel();

        return mappedAddresses;
    }

    public async Task<Address> Get(Guid id)
    {
        var address = await _addressRepository.Get(id);

        if (address == null)
            return null;

        var mappedAddress = address.MapToModel();

        return mappedAddress;
    }

    public async Task<List<Address>> GetByUser(Guid userId)
    {
        var addresses = await _addressRepository.GetByUser(userId);

        var mappedAddresses = addresses.MapToModel();

        return mappedAddresses;
    }

    public async Task Create(Address address)
    {
        try
        {
            var mappedAddress = address.MapToEntity();
            mappedAddress.CreatedAt = DateTimeOffset.UtcNow;
            mappedAddress.UpdatedAt = DateTimeOffset.UtcNow;

            await _addressRepository.Create(mappedAddress);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task Update(Guid id, Address addressDTO)
    {
        var address = await _addressRepository.Get(id);

        if (address == null)
            throw new Exception("Address not found");

        address.UpdatedAt = DateTimeOffset.UtcNow;
        address.Street = addressDTO.Street;
        address.Number = addressDTO.Number;
        address.City = addressDTO.City;
        address.State = addressDTO.State;
        address.ZipCode = addressDTO.ZipCode;

        await _addressRepository.Update(address);
    }

    public async Task Delete(Guid id)
    {
        await _addressRepository.Delete(id);
    }

}
