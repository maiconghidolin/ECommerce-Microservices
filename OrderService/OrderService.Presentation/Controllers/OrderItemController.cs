using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Interfaces;
using OrderService.Application.Models;

namespace OrderService.Presentation.Controllers;

[ApiController]
public class OrderItemController(ILogger<OrderItemController> logger, IOrderItemService orderItemService) : ControllerBase
{
    private readonly ILogger<OrderItemController> _logger = logger;
    private readonly IOrderItemService _orderItemService = orderItemService;

    [HttpGet("orders/{orderId}/itens")]
    public async Task<List<OrderItem>> Get(Guid orderId)
    {
        return await _orderItemService.GetByOrder(orderId);
    }

    [HttpGet("orders/{orderId}/itens/{itemId}")]
    public async Task<ActionResult<OrderItem>> Get(Guid orderId, Guid itemId)
    {
        var orderItem = await _orderItemService.Get(orderId, itemId);

        if (orderItem == null)
            return NotFound(orderItem);

        return orderItem;
    }

    [HttpPost("orders/{orderId}/itens")]
    public async Task<ActionResult> Post(Guid orderId, OrderItem orderItem)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _orderItemService.Create(orderId, orderItem);

        return Created("", orderItem);
    }

    [HttpDelete("orders/{orderId}/itens/{itemId}")]
    public async Task<ActionResult> Delete(Guid orderId, Guid itemId)
    {
        await _orderItemService.Delete(orderId, itemId);
        return NoContent();
    }

}