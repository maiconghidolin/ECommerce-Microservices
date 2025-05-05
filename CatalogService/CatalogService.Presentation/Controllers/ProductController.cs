using CatalogService.Application.Interfaces;
using CatalogService.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Presentation.Controllers;

[ApiController]
[Route("products")]
public class ProductController(ILogger<ProductController> _logger, IProductService _productService) : ControllerBase
{

    [HttpGet()]
    public async Task<List<Product>> Get()
    {
        return await _productService.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> Get(Guid id)
    {
        var product = await _productService.Get(id);

        if (product == null)
            return NotFound(product);

        return product;
    }

    [HttpPost()]
    public async Task<ActionResult> Post(Product product)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _productService.Create(product);

        return Created("", product);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> Patch(Guid id, [FromBody] Product product)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _productService.Update(id, product);

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
        await _productService.Delete(id);
        return NoContent();
    }

}
