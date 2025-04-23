namespace OrderService.Application.Mappers;

public static class OrderMapper
{
    public static Domain.Entities.Order MapToEntity(this Models.Order order)
    {
        return new Domain.Entities.Order
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            ShippingAddressId = order.ShippingAddressId,
            PaymentDataId = order.PaymentDataId,
        };
    }

    public static List<Models.Order> MapToModel(this List<Domain.Entities.Order> orders)
    {
        return orders.Select(o => o.MapToModel()).ToList();
    }

    public static Models.Order MapToModel(this Domain.Entities.Order order)
    {
        return new Models.Order
        {
            Id = order.Id,
            UserId = order.UserId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            ShippingAddressId = order.ShippingAddressId,
            PaymentDataId = order.PaymentDataId,
        };
    }

    public static List<Domain.Entities.Order> MapToEntity(this List<Models.Order> orders)
    {
        return orders.Select(o => o.MapToEntity()).ToList();
    }
}
