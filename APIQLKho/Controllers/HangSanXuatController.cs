using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HangSanXuatController : ControllerBase
    {
        private readonly ILogger<HangSanXuatController> _logger;
        private readonly QlkhohangContext _context;

        public HangSanXuatController(ILogger<HangSanXuatController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các hãng sản xuất, bao gồm các sản phẩm liên quan.
        /// </summary>
        /// <returns>Danh sách các hãng sản xuất.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HangSanXuat>>> Get()
        {
            var manufacturers = await _context.HangSanXuats
                                              .Include(hsx => hsx.SanPhams)
                                              .ToListAsync();
            return Ok(manufacturers);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một hãng sản xuất dựa vào ID.
        /// </summary>
        /// <param name="id">ID của hãng sản xuất cần lấy.</param>
        /// <returns>Thông tin của hãng sản xuất nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<HangSanXuat>> GetById(int id)
        {
            var manufacturer = await _context.HangSanXuats
                                             .Include(hsx => hsx.SanPhams)
                                             .FirstOrDefaultAsync(hsx => hsx.MaHangSanXuat == id);

            if (manufacturer == null)
            {
                return NotFound("Manufacturer not found.");
            }

            return Ok(manufacturer);
        }

        /// <summary>
        /// Tạo mới một hãng sản xuất.
        /// </summary>
        /// <param name="newManufacturer">Thông tin của hãng sản xuất mới cần tạo.</param>
        /// <returns>Hãng sản xuất vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPost]
        public async Task<ActionResult<HangSanXuat>> CreateManufacturer(HangSanXuat newManufacturer)
        {
            if (newManufacturer == null)
            {
                return BadRequest("Manufacturer data is null.");
            }

            _context.HangSanXuats.Add(newManufacturer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newManufacturer.MaHangSanXuat }, newManufacturer);
        }

        /// <summary>
        /// Cập nhật thông tin của một hãng sản xuất dựa vào ID.
        /// </summary>
        /// <param name="id">ID của hãng sản xuất cần cập nhật.</param>
        /// <param name="updatedManufacturer">Thông tin mới của hãng sản xuất.</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateManufacturer(int id, HangSanXuat updatedManufacturer)
        {
            var existingManufacturer = await _context.HangSanXuats.FindAsync(id);
            if (existingManufacturer == null)
            {
                return NotFound("Manufacturer not found.");
            }

            // Cập nhật các thuộc tính
            existingManufacturer.TenHangSanXuat = updatedManufacturer.TenHangSanXuat;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.HangSanXuats.AnyAsync(hsx => hsx.MaHangSanXuat == id))
                {
                    return NotFound("Manufacturer not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa một hãng sản xuất dựa vào ID.
        /// </summary>
        /// <param name="id">ID của hãng sản xuất cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManufacturer(int id)
        {
            var manufacturer = await _context.HangSanXuats.FindAsync(id);
            if (manufacturer == null)
            {
                return NotFound("Manufacturer not found.");
            }

            // Xóa hãng sản xuất
            _context.HangSanXuats.Remove(manufacturer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Tìm kiếm các hãng sản xuất dựa trên từ khóa.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (có thể là tên hãng sản xuất hoặc sản phẩm liên quan).</param>
        /// <returns>Danh sách các hãng sản xuất khớp với từ khóa.</returns>
        // GET: api/hangsanxuat/search/{keyword}
        [HttpGet("{keyword}")]
        public async Task<ActionResult<IEnumerable<HangSanXuat>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var searchResults = await _context.HangSanXuats
                                              .Include(hsx => hsx.SanPhams)
                                              .Where(hsx => hsx.TenHangSanXuat.Contains(keyword) ||
                                                            hsx.SanPhams.Any(sp => sp.TenSanPham.Contains(keyword)))
                                              .ToListAsync();

            return Ok(searchResults);
        }

    }
}
