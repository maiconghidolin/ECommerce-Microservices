using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Interfaces;
using OrderService.Application.Models;

namespace OrderService.Presentation.Controllers;

[ApiController]
[Authorize(Policy = "AdminOrOrderManager")]
[Route("addresses")]
public class AddressController(ILogger<AddressController> _logger, IAddressService _addressService) : ControllerBase
{

    [HttpGet()]
    public async Task<List<Address>> Get()
    {
        return await _addressService.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Address>> Get(Guid id)
    {
        var address = await _addressService.Get(id);

        if (address == null)
            return NotFound(address);

        return address;
    }

    [HttpGet("by-user/{userId}")]
    public async Task<List<Address>> GetByUser(Guid userId)
    {
        return await _addressService.GetByUser(userId);
    }

    [HttpPost()]
    public async Task<ActionResult> Post(Address address)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _addressService.Create(address);

        return Created("", address);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> Patch(Guid id, [FromBody] JsonPatchDocument<Address> patchAddress)
    {
        try
        {
            var address = await _addressService.Get(id);
            if (address == null)
                return NotFound();

            patchAddress.ApplyTo(address, ModelState);

            if (!TryValidateModel(address))
                return BadRequest(ModelState);

            await _addressService.Update(id, address);

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
        await _addressService.Delete(id);
        return NoContent();
    }

}