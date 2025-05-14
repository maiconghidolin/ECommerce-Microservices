using OrderService.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Domain.Entities;

[Table("Orders")]
public class Order : BaseEntity
{

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public OrderStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    public virtual List<OrderItem> OrderItems { get; set; }

    public Guid? ShippingAddressId { get; set; }

    public virtual Address ShippingAddress { get; set; }

    public Guid? PaymentDataId { get; set; }

    public virtual PaymentData PaymentData { get; set; }

}