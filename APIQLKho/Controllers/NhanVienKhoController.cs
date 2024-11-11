using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class NhanVienKhoController : ControllerBase
    {
        private readonly ILogger<NhanVienKhoController> _logger;
        private readonly QlkhohangContext _context;

        public NhanVienKhoController(ILogger<NhanVienKhoController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các nhân viên kho.
        /// </summary>
        /// <returns>Một danh sách các nhân viên kho, bao gồm thông tin kiểm kê liên quan.</returns>
        // GET: api/nhanvienkho
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanVienKho>>> Get()
        {
            var warehouseEmployees = await _context.NhanVienKhos
                                                   .Include(nv => nv.KiemKes) // Bao gồm thông tin kiểm kê
                                                   .ToListAsync();
            return Ok(warehouseEmployees);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một nhân viên kho dựa vào ID.
        /// </summary>
        /// <param name="id">ID của nhân viên kho cần lấy thông tin.</param>
        /// <returns>Thông tin chi tiết của nhân viên kho nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        // GET: api/nhanvienkho/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVienKho>> GetById(int id)
        {
            var warehouseEmployee = await _context.NhanVienKhos
                                                  .Include(nv => nv.KiemKes)
                                                  .FirstOrDefaultAsync(nv => nv.MaNhanVienKho == id);

            if (warehouseEmployee == null)
            {
                return NotFound("Warehouse employee not found.");
            }

            return Ok(warehouseEmployee);
        }

        /// <summary>
        /// Thêm mới một nhân viên kho vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="newEmployee">Thông tin của nhân viên kho mới cần thêm.</param>
        /// <returns>Nhân viên kho vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
        // POST: api/nhanvienkho
        [HttpPost]
        public async Task<ActionResult<NhanVienKho>> CreateWarehouseEmployee(NhanVienKho newEmployee)
        {
            if (newEmployee == null)
            {
                return BadRequest("Warehouse employee data is null.");
            }

            _context.NhanVienKhos.Add(newEmployee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newEmployee.MaNhanVienKho }, newEmployee);
        }

        /// <summary>
        /// Cập nhật thông tin của một nhân viên kho dựa vào ID.
        /// </summary>
        /// <param name="id">ID của nhân viên kho cần cập nhật.</param>
        /// <param name="updatedEmployee">Thông tin nhân viên kho cần cập nhật.</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        // PUT: api/nhanvienkho/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWarehouseEmployee(int id, NhanVienKho updatedEmployee)
        {
            var existingEmployee = await _context.NhanVienKhos.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound("Warehouse employee not found.");
            }

            // Cập nhật các thuộc tính của nhân viên kho
            existingEmployee.TenNhanVien = updatedEmployee.TenNhanVien;
            existingEmployee.Email = updatedEmployee.Email;
            existingEmployee.Sdt = updatedEmployee.Sdt;
            existingEmployee.Hinhanh = updatedEmployee.Hinhanh;
            existingEmployee.NamSinh = updatedEmployee.NamSinh;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.NhanVienKhos.AnyAsync(nv => nv.MaNhanVienKho == id))
                {
                    return NotFound("Warehouse employee not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa một nhân viên kho dựa vào ID.
        /// </summary>
        /// <param name="id">ID của nhân viên kho cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        // DELETE: api/nhanvienkho/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouseEmployee(int id)
        {
            var employee = await _context.NhanVienKhos.FindAsync(id);
            if (employee == null)
            {
                return NotFound("Warehouse employee not found.");
            }

            // Xóa nhân viên kho khỏi cơ sở dữ liệu
            _context.NhanVienKhos.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Tìm kiếm các nhân viên kho dựa trên từ khóa trong tên hoặc email.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (trong tên hoặc email của nhân viên kho).</param>
        /// <returns>Danh sách các nhân viên kho có chứa từ khóa.</returns>
        // GET: api/nhanvienkho/search/{keyword}
        [HttpGet("{keyword}")]
        public async Task<ActionResult<IEnumerable<NhanVienKho>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var searchResults = await _context.NhanVienKhos
                                              .Include(nv => nv.KiemKes) // Bao gồm thông tin kiểm kê
                                              .Where(nv => nv.TenNhanVien.Contains(keyword) || nv.Email.Contains(keyword))
                                              .ToListAsync();

            return Ok(searchResults);
        }

    }
}
