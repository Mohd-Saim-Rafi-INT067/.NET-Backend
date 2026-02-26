using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.Data;
using ProductManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new {Message = $"Product with id {id} not found"});
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest(new {Message="Invalid Product data"});
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductById), new {id = product.Id}, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            if (id != updatedProduct.Id)
            {
                return BadRequest(new {Message="Product ID mismatch"});
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new {Message = $"Product with id {id} not found"});
            }
            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Stock = updatedProduct.Stock;
            product.Description = updatedProduct.Description;

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new {Message = "Product updated successfully", Product = product});
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new {Message = $"Product with id {id} not found"});
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(new {Message = "Product deleted successfully"});
        }
    } 
}