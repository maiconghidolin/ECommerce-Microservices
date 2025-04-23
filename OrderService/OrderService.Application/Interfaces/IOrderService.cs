using OrderService.Application.Models;

namespace OrderService.Application.Interfaces;

public interface IOrderService : ICrudService<Order>
{
    Task SetShippingAddress(Guid id, Guid addressId);

    Task SetPaymentData(Guid id, PaymentData paymentData);
}
