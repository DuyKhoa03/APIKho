using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    //test cccccccccccccccccccccccc
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly QlkhohangContext _context;

        public CategoryController(ILogger<CategoryController> logger, QlkhohangContext qlkhohangContext)
        {
            _logger = logger;
            _context = qlkhohangContext;
        }

        // GET: api/category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            var categories = await _context.Categories
                                           .Where(p => p.Hide == false)
                                           .OrderBy(m => m.Order)
                                           .ToListAsync();
            return Ok(categories);
        }
        // GET: api/category/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            // Tìm kiếm danh mục với CategoryId = id và Hide = false
            var category = await _context.Categories
                                         .Where(p => p.CategoryId == id && p.Hide == false)
                                         .FirstOrDefaultAsync();

            // Nếu không tìm thấy danh mục, trả về NotFound
            if (category == null)
            {
                return NotFound("Category not found.");
            }

            return Ok(category);
        }

        // GET: api/category/search?keyword=value
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Category>>> Search(string keyword)
        {
            var categories = await _context.Categories
                                           .Where(p => p.Hide == false && p.Name.Contains(keyword))
                                           .OrderBy(m => m.Order)
                                           .ToListAsync();
            if (!categories.Any())
            {
                return NotFound("No categories found matching the keyword.");
            }
            return Ok(categories);
        }

        // GET: api/category/paged?pageNumber=1&pageSize=5
        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<Category>>> GetPaged(int pageNumber = 1, int pageSize = 5)
        {
            var categories = await _context.Categories
                                           .Where(p => p.Hide == false)
                                           .OrderBy(m => m.Order)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToListAsync();
            return Ok(categories);
        }

        // POST: api/category
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category newCategory)
        {
            if (newCategory == null)
            {
                return BadRequest("Category data is null.");
            }

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = newCategory.CategoryId }, newCategory);
        }

        // PUT: api/category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id,Category category)
        {
            bool exists = await _context.Categories.AnyAsync(c => c.CategoryId == id);
            if (!exists || id==null)
            {
                return NotFound("Category not found.");
            }
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound("Category not found.");
            }

            // Update các thuộc tính
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.Order = category.Order;
            existingCategory.UpdateDate = category.UpdateDate;
            existingCategory.Hide = category.Hide;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categories.Any(e => e.CategoryId == id))
                {
                    return NotFound("Category not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            // Tìm danh mục cần xóa
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Category not found.");
            }

            // Tìm tất cả các sản phẩm có liên kết với danh mục cần xóa
            var products = await _context.Products
                                         .Where(p => p.Categories.Any(c => c.CategoryId == id))
                                         .ToListAsync();

            // Chuyển CategoryId của các sản phẩm này thành null
            foreach (var product in products)
            {
                product.Categories.Remove(category);
            }

            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();

            // Ẩn danh mục
            category.Hide = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
