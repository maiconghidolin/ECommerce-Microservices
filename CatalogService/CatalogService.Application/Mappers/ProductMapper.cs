namespace CatalogService.Application.Mappers;

public static class ProductMapper
{
    public static Domain.Entities.Product MapToEntity(this Models.Product product)
    {
        return new Domain.Entities.Product
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            UnitPrice = product.UnitPrice
        };
    }

    public static List<Domain.Entities.Product> MapToEntity(this List<Models.Product> products)
    {
        return products.Select(o => o.MapToEntity()).ToList();
    }

    public static Models.Product MapToModel(this Domain.Entities.Product product)
    {
        return new Models.Product
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            UnitPrice = product.UnitPrice
        };
    }

    public static List<Models.Product> MapToModel(this List<Domain.Entities.Product> products)
    {
        return products.Select(o => o.MapToModel()).ToList();
    }

}
