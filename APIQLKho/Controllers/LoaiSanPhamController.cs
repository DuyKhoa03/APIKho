using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoaiSanPhamController : ControllerBase
    {
        private readonly ILogger<LoaiSanPhamController> _logger;
        private readonly QlkhohangContext _context;

        public LoaiSanPhamController(ILogger<LoaiSanPhamController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các loại sản phẩm, bao gồm danh sách các sản phẩm thuộc loại đó.
        /// </summary>
        /// <returns>Danh sách các loại sản phẩm.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoaiSanPham>>> Get()
        {
            var productTypes = await _context.LoaiSanPhams
                                             .Include(ls => ls.SanPhams)
                                             .ToListAsync();
            return Ok(productTypes);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một loại sản phẩm dựa vào ID.
        /// </summary>
        /// <param name="id">ID của loại sản phẩm cần lấy.</param>
        /// <returns>Thông tin của loại sản phẩm nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<LoaiSanPham>> GetById(int id)
        {
            var productType = await _context.LoaiSanPhams
                                            .Include(ls => ls.SanPhams)
                                            .FirstOrDefaultAsync(ls => ls.MaLoaiSanPham == id);

            if (productType == null)
            {
                return NotFound("Product type not found.");
            }

            return Ok(productType);
        }

        /// <summary>
        /// Tạo mới một loại sản phẩm.
        /// </summary>
        /// <param name="newProductType">Thông tin của loại sản phẩm mới cần tạo.</param>
        /// <returns>Loại sản phẩm vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPost]
        public async Task<ActionResult<LoaiSanPham>> CreateProductType(LoaiSanPham newProductType)
        {
            if (newProductType == null)
            {
                return BadRequest("Product type data is null.");
            }

            _context.LoaiSanPhams.Add(newProductType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newProductType.MaLoaiSanPham }, newProductType);
        }

        /// <summary>
        /// Cập nhật thông tin của loại sản phẩm dựa vào ID.
        /// </summary>
        /// <param name="id">ID của loại sản phẩm cần cập nhật.</param>
        /// <param name="updatedProductType">Thông tin mới của loại sản phẩm.</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductType(int id, LoaiSanPham updatedProductType)
        {
            var existingProductType = await _context.LoaiSanPhams.FindAsync(id);
            if (existingProductType == null)
            {
                return NotFound("Product type not found.");
            }

            // Cập nhật các thuộc tính
            existingProductType.TenLoaiSanPham = updatedProductType.TenLoaiSanPham;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.LoaiSanPhams.AnyAsync(ls => ls.MaLoaiSanPham == id))
                {
                    return NotFound("Product type not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa một loại sản phẩm dựa vào ID.
        /// </summary>
        /// <param name="id">ID của loại sản phẩm cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductType(int id)
        {
            var productType = await _context.LoaiSanPhams.FindAsync(id);
            if (productType == null)
            {
                return NotFound("Product type not found.");
            }

            // Xóa loại sản phẩm
            _context.LoaiSanPhams.Remove(productType);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Tìm kiếm loại sản phẩm dựa trên từ khóa trong tên loại sản phẩm.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (trong tên loại sản phẩm).</param>
        /// <returns>Danh sách các loại sản phẩm có chứa từ khóa trong tên.</returns>
        // GET: api/loaisanpham/search/{keyword}
        [HttpGet("{keyword}")]
        public async Task<ActionResult<IEnumerable<LoaiSanPham>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var searchResults = await _context.LoaiSanPhams
                                              .Where(ls => ls.TenLoaiSanPham.Contains(keyword))
                                              .ToListAsync();

            return Ok(searchResults);
        }

    }
}
