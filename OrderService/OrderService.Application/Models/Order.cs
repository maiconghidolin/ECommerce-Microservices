using OrderService.Application.Attributes;
using OrderService.Domain.Enums;

namespace OrderService.Application.Models;

public class Order : BaseModel
{

    [NotEmptyGuid]
    public Guid UserId { get; set; }

    [NotMinDate]
    public DateTimeOffset CreatedAt { get; set; }

    public OrderStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    public Guid? ShippingAddressId { get; set; }

    public Guid? PaymentDataId { get; set; }
}
