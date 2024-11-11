using APIQLKho.Dtos;
using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ChiTietPhieuXuatHangController : ControllerBase
    {
        private readonly ILogger<ChiTietPhieuXuatHangController> _logger;
        private readonly QlkhohangContext _context;

        public ChiTietPhieuXuatHangController(ILogger<ChiTietPhieuXuatHangController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các chi tiết phiếu xuất hàng
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChiTietPhieuXuatHang>>> Get()
        {
            var details = await _context.ChiTietPhieuXuatHangs
                                        .Include(ct => ct.MaSanPhamNavigation)
                                        .Include(ct => ct.MaPhieuXuatHangNavigation)
                                        .ToListAsync();
            return Ok(details);
        }

        /// <summary>
        /// Lấy chi tiết phiếu xuất hàng theo mã phiếu
        /// </summary>
        /// <param name="id">Mã phiếu xuất hàng</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<ChiTietPhieuXuatHang>> GetById(int id)
        {
            var detail = await _context.ChiTietPhieuXuatHangs
                                       .Include(ct => ct.MaSanPhamNavigation)
                                       .Include(ct => ct.MaPhieuXuatHangNavigation)
                                       .FirstOrDefaultAsync(ct => ct.MaPhieuXuatHang == id);

            if (detail == null)
            {
                return NotFound("Detail not found.");
            }

            return Ok(detail);
        }

        /// <summary>
        /// Tạo mới một chi tiết phiếu xuất hàng
        /// </summary>
        /// <param name="detailDto">Dữ liệu chi tiết phiếu xuất hàng cần tạo</param>
        [HttpPost]
        public async Task<ActionResult<ChiTietPhieuXuatHang>> CreateDetail(ChiTietPhieuXuatHangDto detailDto)
        {
            if (detailDto == null)
            {
                return BadRequest("Detail data is null.");
            }

            var newDetail = new ChiTietPhieuXuatHang
            {
                MaSanPham = detailDto.MaSanPham,
                MaPhieuXuatHang = detailDto.MaPhieuXuatHang,
                SoLuong = detailDto.SoLuong,
                DonGiaXuat = detailDto.DonGiaXuat,
                TienMat = detailDto.TienMat,
                NganHang = detailDto.NganHang,
                TrangThai = detailDto.TrangThai,
                Image = detailDto.Image
            };

            _context.ChiTietPhieuXuatHangs.Add(newDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newDetail.MaPhieuXuatHang }, newDetail);
        }

        /// <summary>
        /// Cập nhật một chi tiết phiếu xuất hàng theo mã phiếu
        /// </summary>
        /// <param name="id">Mã phiếu xuất hàng</param>
        /// <param name="detailDto">Dữ liệu chi tiết phiếu xuất hàng cần cập nhật</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDetail(int id, ChiTietPhieuXuatHangDto detailDto)
        {
            if (detailDto == null)
            {
                return BadRequest("Detail data is null.");
            }

            var existingDetail = await _context.ChiTietPhieuXuatHangs.FindAsync(id);
            if (existingDetail == null)
            {
                return NotFound("Detail not found.");
            }

            existingDetail.MaSanPham = detailDto.MaSanPham;
            existingDetail.SoLuong = detailDto.SoLuong;
            existingDetail.DonGiaXuat = detailDto.DonGiaXuat;
            existingDetail.TienMat = detailDto.TienMat;
            existingDetail.NganHang = detailDto.NganHang;
            existingDetail.TrangThai = detailDto.TrangThai;
            existingDetail.Image = detailDto.Image;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Xóa một chi tiết phiếu xuất hàng theo mã phiếu
        /// </summary>
        /// <param name="id">Mã phiếu xuất hàng</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetail(int id)
        {
            var detail = await _context.ChiTietPhieuXuatHangs.FindAsync(id);
            if (detail == null)
            {
                return NotFound("Detail not found.");
            }

            _context.ChiTietPhieuXuatHangs.Remove(detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
