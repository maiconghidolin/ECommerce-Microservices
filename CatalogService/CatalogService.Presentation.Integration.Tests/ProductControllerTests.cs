using CatalogService.Application.Models;
using CatalogService.Presentation.Integration.Tests.Factory;
using System.Net;
using System.Net.Http.Json;

namespace CatalogService.Presentation.Integration.Tests;

public class ProductControllerTests : BaseTests
{

    public ProductControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAll_Should_Return_EmptyList_When_No_Products()
    {
        var response = await _client.GetAsync("/products");

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAll_Should_Return_Product_When_Exists()
    {
        // Arrange
        var product = new Domain.Entities.Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Description = "Test Description",
            UnitPrice = 9.99M,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/products");

        // Assert
        response.EnsureSuccessStatusCode();

        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotEmpty(products);
        Assert.Contains(products, p => p.Id == product.Id);
    }

    [Fact]
    public async Task Get_Should_Return_NotFound_When_NotExists()
    {
        Guid id = Guid.NewGuid();
        var response = await _client.GetAsync($"/products/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_Should_Return_Product_When_Exists()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        var product = new Domain.Entities.Product
        {
            Id = id,
            Name = "Test Product",
            Description = "Test Description",
            UnitPrice = 9.99M,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/products/{id}");

        // Assert
        response.EnsureSuccessStatusCode();

        var productReturned = await response.Content.ReadFromJsonAsync<Product>();

        Assert.Equal(product.Id, productReturned.Id);
        Assert.Equal(product.Name, productReturned.Name);
    }

    [Fact]
    public async Task Post_Should_Return_BadRequest_When_ModelNotValid()
    {
        var product = new Product
        {
            Name = "",
        };

        var response = await _client.PostAsJsonAsync("/products", product);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_Should_Return_Created()
    {
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Desc",
            UnitPrice = 42.00m
        };

        var response = await _client.PostAsJsonAsync("/products", product);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_NoContent()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        var product = new Domain.Entities.Product
        {
            Id = id,
            Name = "Test Product",
            Description = "Test Description",
            UnitPrice = 9.99M,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/products/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    }

}