namespace OrderService.Application.Mappers;

public static class AddressMapper
{
    public static Domain.Entities.Address MapToEntity(this Models.Address address)
    {
        return new Domain.Entities.Address
        {
            Id = address.Id,
            UserId = address.UserId,
            Street = address.Street,
            Number = address.Number,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
        };
    }

    public static List<Domain.Entities.Address> MapToEntity(this List<Models.Address> addresses)
    {
        return addresses.Select(a => a.MapToEntity()).ToList();
    }

    public static Models.Address MapToModel(this Domain.Entities.Address address)
    {
        return new Models.Address
        {
            Id = address.Id,
            UserId = address.UserId,
            Street = address.Street,
            Number = address.Number,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
        };
    }

    public static List<Models.Address> MapToModel(this List<Domain.Entities.Address> addresses)
    {
        return addresses.Select(a => a.MapToModel()).ToList();
    }

}
