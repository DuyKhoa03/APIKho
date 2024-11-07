using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly QlkhohangContext _context;

        public ProductController(ILogger<ProductController> logger, QlkhohangContext qlkhohangContext)
        {
            _logger = logger;
            _context = qlkhohangContext;
        }

        // GET: api/product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            var products = await _context.Products
                                         .Where(p => p.Hide == false)
                                         .OrderBy(p => p.Name)
                                         .ToListAsync();
            return Ok(products);
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null || product.Hide == true)
            {
                return NotFound("Product not found.");
            }

            return Ok(product);
        }

        // GET: api/product/search?keyword=value
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> Search(string keyword)
        {
            var products = await _context.Products
                                         .Where(p => p.Hide == false && p.Name.Contains(keyword))
                                         .OrderBy(p => p.Name)
                                         .ToListAsync();
            if (!products.Any())
            {
                return NotFound("No products found matching the keyword.");
            }
            return Ok(products);
        }

        // GET: api/product/paged?pageNumber=1&pageSize=5
        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<Product>>> GetPaged(int pageNumber = 1, int pageSize = 5)
        {
            var products = await _context.Products
                                         .Where(p => p.Hide == false)
                                         .OrderBy(p => p.Name)
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToListAsync();
            return Ok(products);
        }

        // POST: api/product
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product newProduct)
        {
            if (newProduct == null)
            {
                return BadRequest("Product data is null.");
            }

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newProduct.ProductId }, newProduct);
        }

        // PUT: api/product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product updatedProduct)
        {
            bool exists = await _context.Products.AnyAsync(c => c.ProductId == id);
            if (!exists || id == null)
            {
                return BadRequest("Product not found.");
            }
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            // Update các thuộc tính
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Quantity = updatedProduct.Quantity;
            existingProduct.ExpiryDate = updatedProduct.ExpiryDate;
            existingProduct.UpdateDate = updatedProduct.UpdateDate;
            existingProduct.Hide = updatedProduct.Hide;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.ProductId == id))
                {
                    return NotFound("Product not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            product.Hide = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
