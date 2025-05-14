using OrderService.Application.Attributes;

namespace OrderService.Application.Models;

public class OrderItem : BaseModel
{
    [NotEmptyGuid]
    public Guid OrderId { get; set; }

    [NotEmptyGuid]
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}