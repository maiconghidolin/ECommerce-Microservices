using OrderService.Application.Interfaces;
using OrderService.Application.Mappers;
using OrderService.Application.Models;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces.Repositories;

namespace OrderService.Application.Services;

public class OrderService(IOrderRepository _orderRepository, IPaymentDataRepository _paymentDataRepository) : IOrderService
{

    public async Task<List<Order>> GetAll()
    {
        var orders = await _orderRepository.GetAll();

        var mappedOrders = orders.MapToModel();

        return mappedOrders;
    }

    public async Task<Order> Get(Guid id)
    {
        var order = await _orderRepository.Get(id);

        if (order == null)
            return null;

        var mappedOrder = order.MapToModel();

        return mappedOrder;
    }

    public async Task Create(Order order)
    {
        try
        {
            var mappedOrder = order.MapToEntity();
            mappedOrder.Id = Guid.NewGuid();
            mappedOrder.Status = OrderStatus.Pending;
            mappedOrder.CreatedAt = order.CreatedAt;
            mappedOrder.UpdatedAt = DateTimeOffset.UtcNow;

            await _orderRepository.Create(mappedOrder);

            // send rabbitmq message for order-created
        }
        catch (Exception ex)
        {
            // log
            throw;
        }
    }

    public async Task SetShippingAddress(Guid id, Guid addressId)
    {
        try
        {
            var order = await _orderRepository.Get(id);

            if (order == null)
                throw new Exception("Order not found");

            order.ShippingAddressId = addressId;
            await _orderRepository.Update(order);

            // send rabbitmq message for order-updated
        }
        catch (Exception ex)
        {
            // log
            throw;
        }
    }

    public async Task SetPaymentData(Guid id, PaymentData paymentData)
    {
        try
        {
            var order = await _orderRepository.Get(id);

            if (order == null)
                throw new Exception("Order not found");

            var mappedPaymentData = paymentData.MapToEntity();
            mappedPaymentData.Id = Guid.NewGuid();

            await _paymentDataRepository.Create(mappedPaymentData);

            order.PaymentDataId = mappedPaymentData.Id;
            await _orderRepository.Update(order);

            // send rabbitmq message for order-updated
        }
        catch (Exception ex)
        {
            // log
            throw;
        }
    }

    public async Task Update(Guid id, Order orderDTO)
    {
        var order = await _orderRepository.Get(id);

        if (order == null)
            throw new Exception("Order not found");

        order.UpdatedAt = DateTimeOffset.UtcNow;
        order.ShippingAddressId = orderDTO.ShippingAddressId;
        order.PaymentDataId = orderDTO.PaymentDataId;

        await _orderRepository.Update(order);
    }

    public async Task Delete(Guid id)
    {
        await _orderRepository.Delete(id);
    }

}