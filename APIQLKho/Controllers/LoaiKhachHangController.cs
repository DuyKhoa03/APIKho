using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoaiKhachHangController : ControllerBase
    {
        private readonly ILogger<LoaiKhachHangController> _logger;
        private readonly QlkhohangContext _context;

        public LoaiKhachHangController(ILogger<LoaiKhachHangController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các loại khách hàng, bao gồm danh sách các khách hàng thuộc loại đó.
        /// </summary>
        /// <returns>Danh sách các loại khách hàng.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoaiKhacHang>>> Get()
        {
            var customerTypes = await _context.LoaiKhacHangs
                                              .Include(lk => lk.KhachHangs)
                                              .ToListAsync();
            return Ok(customerTypes);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một loại khách hàng dựa vào ID.
        /// </summary>
        /// <param name="id">ID của loại khách hàng cần lấy.</param>
        /// <returns>Thông tin của loại khách hàng nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<LoaiKhacHang>> GetById(int id)
        {
            var customerType = await _context.LoaiKhacHangs
                                             .Include(lk => lk.KhachHangs)
                                             .FirstOrDefaultAsync(lk => lk.MaLoai == id);

            if (customerType == null)
            {
                return NotFound("Customer type not found.");
            }

            return Ok(customerType);
        }

        /// <summary>
        /// Tạo mới một loại khách hàng.
        /// </summary>
        /// <param name="newCustomerType">Thông tin của loại khách hàng mới cần tạo.</param>
        /// <returns>Loại khách hàng vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPost]
        public async Task<ActionResult<LoaiKhacHang>> CreateCustomerType(LoaiKhacHang newCustomerType)
        {
            if (newCustomerType == null)
            {
                return BadRequest("Customer type data is null.");
            }

            _context.LoaiKhacHangs.Add(newCustomerType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newCustomerType.MaLoai }, newCustomerType);
        }

        /// <summary>
        /// Cập nhật thông tin của loại khách hàng dựa vào ID.
        /// </summary>
        /// <param name="id">ID của loại khách hàng cần cập nhật.</param>
        /// <param name="updatedCustomerType">Thông tin mới của loại khách hàng.</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerType(int id, LoaiKhacHang updatedCustomerType)
        {
            var existingCustomerType = await _context.LoaiKhacHangs.FindAsync(id);
            if (existingCustomerType == null)
            {
                return NotFound("Customer type not found.");
            }

            // Cập nhật các thuộc tính
            existingCustomerType.TenLoai = updatedCustomerType.TenLoai;
            existingCustomerType.ChietKhauXuatHang = updatedCustomerType.ChietKhauXuatHang;
            existingCustomerType.ChiPhiVanChuyen = updatedCustomerType.ChiPhiVanChuyen;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.LoaiKhacHangs.AnyAsync(lk => lk.MaLoai == id))
                {
                    return NotFound("Customer type not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa một loại khách hàng dựa vào ID.
        /// </summary>
        /// <param name="id">ID của loại khách hàng cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerType(int id)
        {
            var customerType = await _context.LoaiKhacHangs.FindAsync(id);
            if (customerType == null)
            {
                return NotFound("Customer type not found.");
            }

            // Xóa loại khách hàng
            _context.LoaiKhacHangs.Remove(customerType);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Tìm kiếm loại khách hàng dựa trên từ khóa trong tên loại khách hàng.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (trong tên loại khách hàng).</param>
        /// <returns>Danh sách các loại khách hàng có chứa từ khóa trong tên.</returns>
        // GET: api/loaikhachhang/search/{keyword}
        [HttpGet("{keyword}")]
        public async Task<ActionResult<IEnumerable<LoaiKhacHang>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var searchResults = await _context.LoaiKhacHangs
                                              .Where(lk => lk.TenLoai.Contains(keyword))
                                              .ToListAsync();

            return Ok(searchResults);
        }

    }
}
