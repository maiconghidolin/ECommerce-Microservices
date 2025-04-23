using OrderService.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Domain.Entities;

[Table("PaymentData")]
public class PaymentData : BaseEntity
{

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    public string CardNumber { get; set; }

}
