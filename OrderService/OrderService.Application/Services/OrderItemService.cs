using OrderService.Application.Interfaces;
using OrderService.Application.Mappers;
using OrderService.Application.Models;
using OrderService.Domain.Interfaces.Repositories;

namespace OrderService.Application.Services;

public class OrderItemService(IOrderItemRepository _orderItemRepository) : IOrderItemService
{
    public async Task<List<OrderItem>> GetByOrder(Guid orderId)
    {
        var items = await _orderItemRepository.GetByOrder(orderId);

        var mappedItems = items.MapToModel();

        return mappedItems;
    }

    public async Task<OrderItem> Get(Guid orderId, Guid id)
    {
        var item = await _orderItemRepository.Get(orderId, id);

        if (item == null)
            return null;

        var mappedItem = item.MapToModel();

        return mappedItem;
    }

    public async Task Create(Guid orderId, OrderItem item)
    {
        try
        {
            var mappedItem = item.MapToEntity();
            mappedItem.Id = Guid.NewGuid();
            mappedItem.OrderId = orderId;
            mappedItem.CreatedAt = DateTimeOffset.UtcNow;
            mappedItem.UpdatedAt = DateTimeOffset.UtcNow;

            await _orderItemRepository.Create(mappedItem);

            // send rabbitmq message for order-created
        }
        catch (Exception ex)
        {
            // log
            throw;
        }
    }

    public async Task Delete(Guid orderId, Guid id)
    {
        await _orderItemRepository.Delete(id);
    }

}
