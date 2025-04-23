namespace OrderService.Domain.Enums;

public enum OrderStatus
{
    Pending,
    ProcessingPayment,
    Paid,
    Shipped,
    Completed,
    Cancelled
}
