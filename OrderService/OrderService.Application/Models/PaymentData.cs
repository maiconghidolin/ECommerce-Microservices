using OrderService.Domain.Enums;

namespace OrderService.Application.Models;

public class PaymentData : BaseModel
{
    public PaymentMethod PaymentMethod { get; set; }

    public string CardNumber { get; set; }

}