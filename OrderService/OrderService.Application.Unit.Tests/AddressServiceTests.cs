using Moq;
using OrderService.Application.Services;
using OrderService.Domain.Interfaces.Repositories;

namespace OrderService.Application.Unit.Tests;

public class AddressServiceTests
{
    private readonly Mock<IAddressRepository> _addressRepository;
    private readonly AddressService _addressService;

    public AddressServiceTests()
    {
        _addressRepository = new Mock<IAddressRepository>();
        _addressService = new AddressService(_addressRepository.Object);
    }

    [Fact]
    public async Task GetAll_Should_Return_ListOfAddresses()
    {
        // Arrange
        var addresses = new List<Domain.Entities.Address>
        {
            new() { Id = Guid.NewGuid(), Street = "Street 1", City = "City 1" },
            new() { Id = Guid.NewGuid(), Street = "Street 2", City = "City 2" }
        };

        _addressRepository.Setup(r => r.GetAll()).ReturnsAsync(addresses);

        // Act
        var result = await _addressService.GetAll();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(addresses.Count, result.Count);
    }

    [Fact]
    public async Task Get_Should_Return_Null_When_NotExists()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        _addressRepository.Setup(r => r.Get(id)).ReturnsAsync((Domain.Entities.Address)null);

        // Act
        var result = await _addressService.Get(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Get_Should_Return_Address()
    {
        // Arrange
        var address = new Domain.Entities.Address
        {
            Id = Guid.NewGuid(),
            Street = "Street 1",
            Number = "123",
            City = "City 1",
            State = "State 1",
            ZipCode = "12345",
        };

        _addressRepository.Setup(r => r.Get(address.Id)).ReturnsAsync(address);

        // Act
        var result = await _addressService.Get(address.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(address.Id, result.Id);
        Assert.Equal(address.Street, result.Street);
        Assert.Equal(address.Number, result.Number);
        Assert.Equal(address.City, result.City);
        Assert.Equal(address.State, result.State);
        Assert.Equal(address.ZipCode, result.ZipCode);
    }

    [Fact]
    public async Task GetByUser_Should_Return_ListOfAddresses()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        var addresses = new List<Domain.Entities.Address>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, Street = "Street 1", City = "City 1" },
            new() { Id = Guid.NewGuid(), UserId = userId, Street = "Street 2", City = "City 2" }
        };

        _addressRepository.Setup(r => r.GetByUser(userId)).ReturnsAsync(addresses);

        // Act
        var result = await _addressService.GetByUser(userId);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(addresses.Count, result.Count);
    }

    [Fact]
    public async Task GetByUser_Should_Return_Empty_When_NotExists()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        _addressRepository.Setup(r => r.GetByUser(id)).ReturnsAsync([]);

        // Act
        var result = await _addressService.GetByUser(id);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Create_Should_Call_Repository()
    {
        // Arrange
        var address = new Models.Address
        {
            Id = Guid.NewGuid(),
            Street = "Street 1",
            Number = "123",
            City = "City 1",
            State = "State 1",
            ZipCode = "12345",
        };

        // Act
        await _addressService.Create(address);

        // Assert
        _addressRepository.Verify(r => r.Create(It.IsAny<Domain.Entities.Address>()), Times.Once);
    }

    [Fact]
    public async Task Update_Should_Throw_Exception_When_NotFound()
    {
        // Arrange
        var addressId = Guid.NewGuid();

        _addressRepository.Setup(r => r.Get(addressId)).ReturnsAsync((Domain.Entities.Address)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _addressService.Update(addressId, new Models.Address()));
    }

    [Fact]
    public async Task Update_Should_Call_Repository()
    {
        // Arrange
        var address = new Domain.Entities.Address
        {
            Id = Guid.NewGuid(),
            Street = "Street 1",
            Number = "123",
            City = "City 1",
            State = "State 1",
            ZipCode = "12345",
        };

        _addressRepository.Setup(r => r.Get(address.Id)).ReturnsAsync(address);

        // Act
        await _addressService.Update(address.Id, new Models.Address());

        // Assert
        _addressRepository.Verify(r => r.Update(It.IsAny<Domain.Entities.Address>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_Call_Repository()
    {
        // Arrange
        var addressId = Guid.NewGuid();

        // Act
        await _addressService.Delete(addressId);

        // Assert
        _addressRepository.Verify(r => r.Delete(addressId), Times.Once);
    }

}