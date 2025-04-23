using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Domain.Entities;

[Table("OrderItems")]
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }

    public virtual Order Order { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

}