using CatalogService.Application.Interfaces;
using CatalogService.Application.Mappers;
using CatalogService.Application.Models;
using CatalogService.Domain.Interfaces.Repositories;

namespace CatalogService.Application.Services;

public class ProductService(IProductRepository _productRepository) : IProductService
{

    public async Task<List<Product>> GetAll()
    {
        var products = await _productRepository.GetAll();

        var mappedProducts = products.MapToModel();

        return mappedProducts;
    }

    public async Task<Product> Get(Guid id)
    {
        var product = await _productRepository.Get(id);

        if (product == null)
            return null;

        var mappedProduct = product.MapToModel();

        return mappedProduct;
    }

    public async Task Create(Product product)
    {
        var mappedProduct = product.MapToEntity();
        mappedProduct.Id = product.Id != Guid.Empty ? product.Id : Guid.NewGuid();
        mappedProduct.CreatedAt = DateTimeOffset.UtcNow;
        mappedProduct.UpdatedAt = DateTimeOffset.UtcNow;

        await _productRepository.Create(mappedProduct);

        // send rabbitmq message for product-created
    }

    public async Task Update(Guid id, Product productDTO)
    {
        var product = await _productRepository.Get(id);

        if (product == null)
            throw new Exception("Product not found");

        product.UpdatedAt = DateTimeOffset.UtcNow;
        product.Name = productDTO.Name;
        product.Description = productDTO.Description;
        product.UnitPrice = productDTO.UnitPrice;

        await _productRepository.Update(product);
    }

    public async Task Delete(Guid id)
    {
        await _productRepository.Delete(id);
    }

}
