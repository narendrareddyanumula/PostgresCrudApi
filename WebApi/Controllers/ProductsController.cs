
// WebApi/Controllers/ProductsController.cs
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> PostProduct(ProductDto productDto)
    {
        await _productService.AddAsync(productDto);
        return CreatedAtAction(nameof(GetProduct), new { id = productDto.Id }, productDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int id, ProductDto productDto)
    {
        if (id != productDto.Id)
        {
            return BadRequest();
        }

        await _productService.UpdateAsync(productDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}
