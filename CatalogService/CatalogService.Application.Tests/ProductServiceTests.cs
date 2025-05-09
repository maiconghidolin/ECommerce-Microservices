using CatalogService.Application.Services;
using CatalogService.Domain.Interfaces.Repositories;
using Moq;

namespace CatalogService.Application.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepository;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepository = new Mock<IProductRepository>();
        _productService = new ProductService(_productRepository.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnListOfProducts()
    {
        // Arrange
        var expectedProducts = new List<Domain.Entities.Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Product1", Description = "Description1", UnitPrice = 10.0m },
            new() { Id = Guid.NewGuid(), Name = "Product2", Description = "Description2", UnitPrice = 20.0m }
        };

        _productRepository.Setup(r => r.GetAll())
                          .ReturnsAsync(expectedProducts);

        // Act
        var result = await _productService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProducts.Count, result.Count);
    }

    [Fact]
    public async Task Get_WhenExists_ShouldReturnProduct()
    {
        // Arrange
        Domain.Entities.Product product = new() { Id = Guid.NewGuid(), Name = "Product1", Description = "Description1", UnitPrice = 10.0m };
        Guid guid = Guid.NewGuid();

        _productRepository.Setup(r => r.Get(guid))
                          .ReturnsAsync(product);

        // Act
        var result = await _productService.Get(guid);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Description, result.Description);
        Assert.Equal(product.UnitPrice, result.UnitPrice);
    }

    [Fact]
    public async Task Get_WhenNotExists_ShouldReturnNull()
    {
        // Arrange
        Guid guid = Guid.NewGuid();

        _productRepository.Setup(r => r.Get(guid))
                          .ReturnsAsync((Domain.Entities.Product)null);

        // Act
        var result = await _productService.Get(guid);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Create_ShouldCallRepository()
    {
        var product = new Models.Product { Name = "New", Description = "New", UnitPrice = 15 };

        await _productService.Create(product);

        _productRepository.Verify(r => r.Create(It.IsAny<Domain.Entities.Product>()), Times.Once);
    }

    [Fact]
    public async Task Update_WhenProductDoesNotExists_ShouldRaiseAnException()
    {
        Guid guid = Guid.NewGuid();
        var product = new Models.Product { Name = "New", Description = "New", UnitPrice = 15 };

        _productRepository.Setup(r => r.Get(guid))
                          .ReturnsAsync((Domain.Entities.Product)null);

        await Assert.ThrowsAsync<Exception>(async () => await _productService.Update(guid, product));
    }

    [Fact]
    public async Task Update_WhenProductExists_ShouldCallRepository()
    {
        var productDTO = new Models.Product { Name = "New", Description = "New", UnitPrice = 15 };
        Domain.Entities.Product product = new() { Id = Guid.NewGuid(), Name = "Product1", Description = "Description1", UnitPrice = 10.0m };
        Guid guid = Guid.NewGuid();

        _productRepository.Setup(r => r.Get(guid))
                          .ReturnsAsync(product);

        await _productService.Update(guid, productDTO);

        _productRepository.Verify(r => r.Update(It.IsAny<Domain.Entities.Product>()), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldCallRepository()
    {
        await _productService.Delete(Guid.NewGuid());

        _productRepository.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Once);
    }
}
