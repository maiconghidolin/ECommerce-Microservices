using Microsoft.Extensions.Configuration;
using OrderService.Application.Interfaces;
using OrderService.Application.Mappers;
using OrderService.Application.Models;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces.Repositories;
using System.Net.Http.Json;

namespace OrderService.Application.Services;

public class OrderService(IOrderRepository _orderRepository, IPaymentDataRepository _paymentDataRepository, IHttpClientFactory _httpClientFactory, IConfiguration _configuration) : IOrderService
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
        var mappedOrder = order.MapToEntity();
        mappedOrder.Id = Guid.NewGuid();
        mappedOrder.Status = OrderStatus.Pending;
        mappedOrder.CreatedAt = order.CreatedAt;
        mappedOrder.UpdatedAt = DateTimeOffset.UtcNow;

        await _orderRepository.Create(mappedOrder);

        // send rabbitmq message for order-created
    }

    public async Task SetShippingAddress(Guid id, Guid addressId)
    {
        var order = await _orderRepository.Get(id);

        if (order == null)
            throw new Exception("Order not found");

        order.ShippingAddressId = addressId;
        await _orderRepository.Update(order);

        // send rabbitmq message for order-updated
    }

    public async Task SetPaymentData(Guid id, PaymentData paymentData)
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

    public async Task TracingTest(bool raiseError)
    {
        var catalogClientName = _configuration["CatalogService:ClientName"];
        var catalogClient = _httpClientFactory.CreateClient(catalogClientName);

        var product = new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Description = "Test Product",
            UnitPrice = 10
        };

        var content = JsonContent.Create(product);

        var httpResponse = await catalogClient.PostAsync("products", content);

        httpResponse.EnsureSuccessStatusCode();

        if (raiseError)
            throw new Exception("Tracing test raised error");

        httpResponse = await catalogClient.DeleteAsync($"products/{product.Id}");

        httpResponse.EnsureSuccessStatusCode();

    }

}