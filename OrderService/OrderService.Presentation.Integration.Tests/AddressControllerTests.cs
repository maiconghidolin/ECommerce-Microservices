using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OrderService.Application.Models;
using OrderService.Infrastructure;
using OrderService.Presentation.Integration.Tests.Factory;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace OrderService.Presentation.Integration.Tests;

public class AddressControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient _client;
    protected readonly EFContext _dbContext;

    public AddressControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<EFContext>();
    }

    [Fact]
    public async Task GetAll_Should_Return_EmptyList_When_NoAddresses()
    {
        // Arrange
        var allAddresses = _dbContext.Addresses.ToList();
        _dbContext.Addresses.RemoveRange(allAddresses);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/addresses");

        // Assert
        response.EnsureSuccessStatusCode();

        var addresses = await response.Content.ReadFromJsonAsync<List<Address>>();

        Assert.Empty(addresses);
    }

    [Fact]
    public async Task GetAll_Should_Return_Address_When_Exists()
    {
        // Arrange
        var address = new Domain.Entities.Address
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Street = "123 Main St",
            Number = "Apt 4B",
            City = "Anytown",
            State = "CA",
            ZipCode = "12345",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Addresses.Add(address);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/addresses");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<Address>>();

        Assert.NotEmpty(result);
        Assert.Contains(result, a => a.Id == address.Id);
        Assert.Contains(result, a => a.UserId == address.UserId);
    }

    [Fact]
    public async Task Get_Should_Return_NotFound_When_NotExists()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/addresses/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_Should_Return_Address_When_Exists()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        var address = new Domain.Entities.Address
        {
            Id = id,
            UserId = Guid.NewGuid(),
            Street = "123 Main St",
            Number = "Apt 4B",
            City = "Anytown",
            State = "CA",
            ZipCode = "12345",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Addresses.Add(address);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/addresses/{id}");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Address>();

        Assert.NotNull(result);
        Assert.Equal(address.Id, result.Id);
    }

    [Fact]
    public async Task GetByUser_Should_Return_Addresses_When_Exists()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        var address = new Domain.Entities.Address
        {
            Id = id,
            UserId = userId,
            Street = "123 Main St",
            Number = "Apt 4B",
            City = "Anytown",
            State = "CA",
            ZipCode = "12345",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Addresses.Add(address);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/addresses/by-user/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<Address>>();

        Assert.NotEmpty(result);
        Assert.True(result.Count > 0);
        Assert.Contains(result, a => a.Id == address.Id);
        Assert.Contains(result, a => a.UserId == address.UserId);
    }

    [Fact]
    public async Task Post_Should_Return_BadRequest_When_InvalidModel()
    {
        // Arrange
        var address = new Address
        {
            Id = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/addresses", address);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_Should_Return_Created()
    {
        // Arrange
        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Street = "123 Main St",
            Number = "Apt 4B",
            City = "Anytown",
            State = "CA",
            ZipCode = "12345",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/addresses", address);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Patch_Should_Return_NotFound_When_NotExists()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        var patchDoc = new JsonPatchDocument<Address>();
        patchDoc.Replace(a => a.Street, "abc");
        patchDoc.Replace(a => a.Number, "def");

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(patchDoc), Encoding.UTF8, "application/json-patch+json");
        var response = await _client.PatchAsync($"/addresses/{id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Patch_Should_Return_BadRequest_When_InvalidModel()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        var address = new Domain.Entities.Address
        {
            Id = id,
            UserId = Guid.NewGuid(),
            Street = "123 Main St",
            Number = "Apt 4B",
            City = "Anytown",
            State = "CA",
            ZipCode = "12345",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Addresses.Add(address);
        await _dbContext.SaveChangesAsync();

        var patchDoc = new JsonPatchDocument<Address>();
        patchDoc.Replace(a => a.Street, null);
        patchDoc.Replace(a => a.Number, "");

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(patchDoc), Encoding.UTF8, "application/json-patch+json");
        var response = await _client.PatchAsync($"/addresses/{id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Patch_Should_Return_Ok_When_ValidModel()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        var address = new Domain.Entities.Address
        {
            Id = id,
            UserId = Guid.NewGuid(),
            Street = "123 Main St",
            Number = "Apt 4B",
            City = "Anytown",
            State = "CA",
            ZipCode = "12345",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Addresses.Add(address);
        await _dbContext.SaveChangesAsync();

        var patchDoc = new JsonPatchDocument<Address>();
        patchDoc.Replace(a => a.Street, "456 Elm St");
        patchDoc.Replace(a => a.Number, "Apt 5C");
        patchDoc.Replace(a => a.City, "Othertown");
        patchDoc.Replace(a => a.State, "NY");
        patchDoc.Replace(a => a.ZipCode, "54321");

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(patchDoc), Encoding.UTF8, "application/json-patch+json");
        var response = await _client.PatchAsync($"/addresses/{id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_NoContent()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        var address = new Domain.Entities.Address
        {
            Id = id,
            UserId = Guid.NewGuid(),
            Street = "123 Main St",
            Number = "Apt 4B",
            City = "Anytown",
            State = "CA",
            ZipCode = "12345",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Addresses.Add(address);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/addresses/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

}

