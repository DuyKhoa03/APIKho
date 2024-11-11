using APIQLKho.Dtos;
using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ChiTietKiemKeController : ControllerBase
    {
        private readonly ILogger<ChiTietKiemKeController> _logger;
        private readonly QlkhohangContext _context;

        public ChiTietKiemKeController(ILogger<ChiTietKiemKeController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các chi tiết kiểm kê
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChiTietKiemKe>>> Get()
        {
            var details = await _context.ChiTietKiemKes
                                        .Include(ct => ct.MaSanPhamNavigation)
                                        .Include(ct => ct.MaKiemKeNavigation)
                                        .ToListAsync();
            return Ok(details);
        }

        /// <summary>
        /// Lấy chi tiết kiểm kê theo mã kiểm kê
        /// </summary>
        /// <param name="id">Mã kiểm kê</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<ChiTietKiemKe>> GetById(int id)
        {
            var detail = await _context.ChiTietKiemKes
                                       .Include(ct => ct.MaSanPhamNavigation)
                                       .Include(ct => ct.MaKiemKeNavigation)
                                       .FirstOrDefaultAsync(ct => ct.MaKiemKe == id);

            if (detail == null)
            {
                return NotFound("Detail not found.");
            }

            return Ok(detail);
        }

        /// <summary>
        /// Tạo mới một chi tiết kiểm kê
        /// </summary>
        /// <param name="detailDto">Dữ liệu chi tiết kiểm kê cần tạo</param>
        [HttpPost]
        public async Task<ActionResult<ChiTietKiemKe>> CreateDetail(ChiTietKiemKeDto detailDto)
        {
            if (detailDto == null)
            {
                return BadRequest("Detail data is null.");
            }

            var newDetail = new ChiTietKiemKe
            {
                MaSanPham = detailDto.MaSanPham,
                MaKiemKe = detailDto.MaKiemKe,
                SoLuongTon = detailDto.SoLuongTon,
                TrangThai = detailDto.TrangThai
            };

            _context.ChiTietKiemKes.Add(newDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newDetail.MaKiemKe }, newDetail);
        }

        /// <summary>
        /// Cập nhật một chi tiết kiểm kê theo mã kiểm kê
        /// </summary>
        /// <param name="id">Mã kiểm kê</param>
        /// <param name="detailDto">Dữ liệu chi tiết kiểm kê cần cập nhật</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDetail(int id, ChiTietKiemKeDto detailDto)
        {
            if (detailDto == null)
            {
                return BadRequest("Detail data is null.");
            }

            var existingDetail = await _context.ChiTietKiemKes.FindAsync(id);
            if (existingDetail == null)
            {
                return NotFound("Detail not found.");
            }

            existingDetail.MaSanPham = detailDto.MaSanPham;
            existingDetail.SoLuongTon = detailDto.SoLuongTon;
            existingDetail.TrangThai = detailDto.TrangThai;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Xóa một chi tiết kiểm kê theo mã kiểm kê
        /// </summary>
        /// <param name="id">Mã kiểm kê</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetail(int id)
        {
            var detail = await _context.ChiTietKiemKes.FindAsync(id);
            if (detail == null)
            {
                return NotFound("Detail not found.");
            }

            _context.ChiTietKiemKes.Remove(detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
