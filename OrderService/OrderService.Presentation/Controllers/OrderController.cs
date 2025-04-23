using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Interfaces;
using OrderService.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Presentation.Controllers;

[ApiController]
[Route("orders")]
public class OrderController(ILogger<OrderController> logger, IOrderService orderService) : ControllerBase
{
    private readonly ILogger<OrderController> _logger = logger;
    private readonly IOrderService _orderService = orderService;

    [HttpGet()]
    public async Task<List<Order>> Get()
    {
        return await _orderService.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> Get(Guid id)
    {
        var order = await _orderService.Get(id);

        if (order == null)
            return NotFound(order);

        return order;
    }

    [HttpPost()]
    public async Task<ActionResult> Post(Order order)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _orderService.Create(order);

        return Created("", order);
    }

    [HttpPost("{id}/set-shipping-address")]
    public async Task<ActionResult> Patch(Guid id, [FromBody, Required] Guid addressId)
    {
        await _orderService.SetShippingAddress(id, addressId);
        return Ok();
    }

    [HttpPost("{id}/set-payment-data")]
    public async Task<ActionResult> Patch(Guid id, [FromBody] PaymentData paymentData)
    {
        await _orderService.SetPaymentData(id, paymentData);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _orderService.Delete(id);
        return NoContent();
    }

}