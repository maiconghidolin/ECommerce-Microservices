using OrderService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.Models;

public class Order : BaseModel
{

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; }

    public OrderStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    public Guid ShippingAddressId { get; set; }

    public Guid PaymentDataId { get; set; }
}
