using APIQLKho.Dtos;
using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ChiTietPhieuNhapHangController : ControllerBase
    {
        private readonly ILogger<ChiTietPhieuNhapHangController> _logger;
        private readonly QlkhohangContext _context;

        public ChiTietPhieuNhapHangController(ILogger<ChiTietPhieuNhapHangController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các chi tiết phiếu nhập hàng
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChiTietPhieuNhapHang>>> Get()
        {
            var details = await _context.ChiTietPhieuNhapHangs
                                        .Include(ct => ct.MaSanPhamNavigation)
                                        .Include(ct => ct.MaPhieuNhapHangNavigation)
                                        .ToListAsync();
            return Ok(details);
        }

        /// <summary>
        /// Lấy chi tiết phiếu nhập hàng theo mã phiếu
        /// </summary>
        /// <param name="id">Mã phiếu nhập hàng</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<ChiTietPhieuNhapHang>> GetById(int id)
        {
            var detail = await _context.ChiTietPhieuNhapHangs
                                       .Include(ct => ct.MaSanPhamNavigation)
                                       .Include(ct => ct.MaPhieuNhapHangNavigation)
                                       .FirstOrDefaultAsync(ct => ct.MaPhieuNhapHang == id);

            if (detail == null)
            {
                return NotFound("Detail not found.");
            }

            return Ok(detail);
        }

        /// <summary>
        /// Tạo mới một chi tiết phiếu nhập hàng
        /// </summary>
        /// <param name="detailDto">Dữ liệu chi tiết phiếu nhập hàng cần tạo</param>
        [HttpPost]
        public async Task<ActionResult<ChiTietPhieuNhapHang>> CreateDetail(ChiTietPhieuNhapHangDto detailDto)
        {
            if (detailDto == null)
            {
                return BadRequest("Detail data is null.");
            }

            var newDetail = new ChiTietPhieuNhapHang
            {
                MaSanPham = detailDto.MaSanPham,
                MaPhieuNhapHang = detailDto.MaPhieuNhapHang,
                SoLuong = detailDto.SoLuong,
                DonGiaNhap = detailDto.DonGiaNhap,
                TrangThai = detailDto.TrangThai,
                Image = detailDto.Image
            };

            _context.ChiTietPhieuNhapHangs.Add(newDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newDetail.MaPhieuNhapHang }, newDetail);
        }

        /// <summary>
        /// Cập nhật một chi tiết phiếu nhập hàng theo mã phiếu
        /// </summary>
        /// <param name="id">Mã phiếu nhập hàng</param>
        /// <param name="detailDto">Dữ liệu chi tiết phiếu nhập hàng cần cập nhật</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDetail(int id, ChiTietPhieuNhapHangDto detailDto)
        {
            if (detailDto == null)
            {
                return BadRequest("Detail data is null.");
            }

            var existingDetail = await _context.ChiTietPhieuNhapHangs.FindAsync(id);
            if (existingDetail == null)
            {
                return NotFound("Detail not found.");
            }

            existingDetail.MaSanPham = detailDto.MaSanPham;
            existingDetail.SoLuong = detailDto.SoLuong;
            existingDetail.DonGiaNhap = detailDto.DonGiaNhap;
            existingDetail.TrangThai = detailDto.TrangThai;
            existingDetail.Image = detailDto.Image;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Xóa một chi tiết phiếu nhập hàng theo mã phiếu
        /// </summary>
        /// <param name="id">Mã phiếu nhập hàng</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetail(int id)
        {
            var detail = await _context.ChiTietPhieuNhapHangs.FindAsync(id);
            if (detail == null)
            {
                return NotFound("Detail not found.");
            }

            _context.ChiTietPhieuNhapHangs.Remove(detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
