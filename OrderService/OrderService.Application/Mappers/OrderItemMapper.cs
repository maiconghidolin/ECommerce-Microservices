namespace OrderService.Application.Mappers;

public static class OrderItemMapper
{
    public static Domain.Entities.OrderItem MapToEntity(this Models.OrderItem orderItem)
    {
        return new Domain.Entities.OrderItem
        {
            Id = orderItem.Id,
            OrderId = orderItem.OrderId,
            ProductId = orderItem.ProductId,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice
        };
    }

    public static List<Domain.Entities.OrderItem> MapToEntity(this List<Models.OrderItem> orderItems)
    {
        return orderItems.Select(o => o.MapToEntity()).ToList();
    }

    public static Models.OrderItem MapToModel(this Domain.Entities.OrderItem orderItem)
    {
        return new Models.OrderItem
        {
            Id = orderItem.Id,
            OrderId = orderItem.OrderId,
            ProductId = orderItem.ProductId,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice
        };
    }

    public static List<Models.OrderItem> MapToModel(this List<Domain.Entities.OrderItem> orderItems)
    {
        return orderItems.Select(o => o.MapToModel()).ToList();
    }

}
