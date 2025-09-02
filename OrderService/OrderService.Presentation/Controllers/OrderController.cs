using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Interfaces;
using OrderService.Application.Models;

namespace OrderService.Presentation.Controllers;

[ApiController]
[Route("orders")]
public class OrderController(ILogger<OrderController> _logger, IOrderService _orderService) : ControllerBase
{

    [HttpGet("test")]
    public IActionResult Test()
    {
        _logger.LogInformation("Test endpoint");
        return Ok();
    }

    [HttpPost("tracing-test")]
    public async Task<ActionResult> TracingTest([FromBody] bool raiseError = false)
    {
        _logger.LogInformation("Tracing test started");

        await _orderService.TracingTest(raiseError);

        _logger.LogInformation("Tracing test finished");

        return Ok();
    }

    [HttpGet()]
    public async Task<List<Order>> Get()
    {
        _logger.LogInformation("Getting all orders");
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
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _orderService.Create(order);

            return Created("", order);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/set-shipping-address")]
    public async Task<ActionResult> SetShippingAddress(Guid id, [FromBody] Guid addressId)
    {
        try
        {
            await _orderService.SetShippingAddress(id, addressId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/set-payment-data")]
    public async Task<ActionResult> SetPaymentData(Guid id, [FromBody] PaymentData paymentData)
    {
        try
        {
            await _orderService.SetPaymentData(id, paymentData);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _orderService.Delete(id);
        return NoContent();
    }

}