using APIQLKho.Dtos;
using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KiemKeController : ControllerBase
    {
        private readonly ILogger<KiemKeController> _logger;
        private readonly QlkhohangContext _context;

        public KiemKeController(ILogger<KiemKeController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các phiếu kiểm kê, bao gồm thông tin nhân viên kho và chi tiết kiểm kê.
        /// </summary>
        /// <returns>Danh sách các phiếu kiểm kê.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KiemKe>>> Get()
        {
            var inventoryChecks = await _context.KiemKes
                                                .Include(k => k.MaNhanVienKhoNavigation)
                                                .Include(k => k.ChiTietKiemKes)
                                                .ToListAsync();
            return Ok(inventoryChecks);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một phiếu kiểm kê dựa vào ID.
        /// </summary>
        /// <param name="id">ID của phiếu kiểm kê cần lấy.</param>
        /// <returns>Thông tin của phiếu kiểm kê nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<KiemKe>> GetById(int id)
        {
            var inventoryCheck = await _context.KiemKes
                                               .Include(k => k.MaNhanVienKhoNavigation)
                                               .Include(k => k.ChiTietKiemKes)
                                               .FirstOrDefaultAsync(k => k.MaKiemKe == id);

            if (inventoryCheck == null)
            {
                return NotFound("Inventory check not found.");
            }

            return Ok(inventoryCheck);
        }

        /// <summary>
        /// Tạo mới một phiếu kiểm kê.
        /// </summary>
        /// <param name="newInventoryCheckDto">Thông tin của phiếu kiểm kê mới cần tạo.</param>
        /// <returns>Phiếu kiểm kê vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPost]
        public async Task<ActionResult<KiemKe>> CreateInventoryCheck(KiemKeDto newInventoryCheckDto)
        {
            if (newInventoryCheckDto == null)
            {
                return BadRequest("Inventory check data is null.");
            }

            var newInventoryCheck = new KiemKe
            {
                MaNhanVienKho = newInventoryCheckDto.MaNhanVienKho,
                NgayKiemKe = newInventoryCheckDto.NgayKiemKe
            };

            _context.KiemKes.Add(newInventoryCheck);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newInventoryCheck.MaKiemKe }, newInventoryCheck);
        }

        /// <summary>
        /// Cập nhật thông tin của một phiếu kiểm kê dựa vào ID.
        /// </summary>
        /// <param name="id">ID của phiếu kiểm kê cần cập nhật.</param>
        /// <param name="updatedInventoryCheckDto">Thông tin mới của phiếu kiểm kê.</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventoryCheck(int id, KiemKeDto updatedInventoryCheckDto)
        {
            if (updatedInventoryCheckDto == null)
            {
                return BadRequest("Inventory check data is null.");
            }

            var existingInventoryCheck = await _context.KiemKes.FindAsync(id);
            if (existingInventoryCheck == null)
            {
                return NotFound("Inventory check not found.");
            }

            // Cập nhật các thuộc tính
            existingInventoryCheck.MaNhanVienKho = updatedInventoryCheckDto.MaNhanVienKho;
            existingInventoryCheck.NgayKiemKe = updatedInventoryCheckDto.NgayKiemKe;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.KiemKes.AnyAsync(k => k.MaKiemKe == id))
                {
                    return NotFound("Inventory check not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa một phiếu kiểm kê dựa vào ID.
        /// </summary>
        /// <param name="id">ID của phiếu kiểm kê cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryCheck(int id)
        {
            var inventoryCheck = await _context.KiemKes.FindAsync(id);
            if (inventoryCheck == null)
            {
                return NotFound("Inventory check not found.");
            }

            // Xóa kiểm kê
            _context.KiemKes.Remove(inventoryCheck);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Tìm kiếm các phiếu kiểm kê dựa trên từ khóa.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (có thể là ngày kiểm kê hoặc mã nhân viên kho).</param>
        /// <returns>Danh sách các phiếu kiểm kê khớp với từ khóa.</returns>
        // GET: api/kiemke/search/{keyword}
        [HttpGet("{keyword}")]
        public async Task<ActionResult<IEnumerable<KiemKe>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var searchResults = await _context.KiemKes
                                              .Include(k => k.MaNhanVienKhoNavigation)
                                              .Include(k => k.ChiTietKiemKes)
                                              .Where(k => k.NgayKiemKe.ToString().Contains(keyword) ||
                                                          k.MaNhanVienKhoNavigation.TenNhanVien.Contains(keyword))
                                              .ToListAsync();

            return Ok(searchResults);
        }

    }
}
