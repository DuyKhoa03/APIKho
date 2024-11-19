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
        public async Task<ActionResult<IEnumerable<ChiTietPhieuXuatHangDto>>> Get()
        {
            var details = await _context.ChiTietPhieuXuatHangs
                                        .Include(ct => ct.MaSanPhamNavigation)
                                        .Include(ct => ct.MaPhieuXuatHangNavigation)
                                        .Select(ct => new ChiTietPhieuXuatHangDto
                                        {
                                            MaSanPham = ct.MaSanPham,
                                            TenSanPham = ct.MaSanPhamNavigation.TenSanPham,
                                            MaPhieuXuatHang = ct.MaPhieuXuatHang,
                                            SoLuong = ct.SoLuong,
                                            DonGiaXuat = ct.DonGiaXuat,
                                            TienMat = ct.TienMat,
                                            NganHang = ct.NganHang,
                                            TrangThai = ct.TrangThai,
                                            Image = ct.Image // Đường dẫn ảnh nếu có
                                        })
                                        .ToListAsync();

            return Ok(details);
        }

        /// <summary>
        /// Lấy chi tiết phiếu xuất hàng theo mã phiếu
        /// </summary>
        /// <param name="id">Mã phiếu xuất hàng</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<ChiTietPhieuXuatHangDto>> GetById(int id)
        {
            var detail = await _context.ChiTietPhieuXuatHangs
                                       .Include(ct => ct.MaSanPhamNavigation)
                                       .Include(ct => ct.MaPhieuXuatHangNavigation)
                                       .Where(ct => ct.MaPhieuXuatHang == id)
                                       .Select(ct => new ChiTietPhieuXuatHangDto
                                       {
                                           MaSanPham = ct.MaSanPham,
                                           TenSanPham = ct.MaSanPhamNavigation.TenSanPham,
                                           MaPhieuXuatHang = ct.MaPhieuXuatHang,
                                           SoLuong = ct.SoLuong,
                                           DonGiaXuat = ct.DonGiaXuat,
                                           TienMat = ct.TienMat,
                                           NganHang = ct.NganHang,
                                           TrangThai = ct.TrangThai,
                                           Image = ct.Image
                                       })
                                       .FirstOrDefaultAsync();

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
        [Route("uploadfile")]
        public async Task<ActionResult<ChiTietPhieuXuatHang>> CreateDetailWithImage([FromForm] ChiTietPhieuXuatHangDto detailDto)
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
                TrangThai = detailDto.TrangThai
            };

            // Xử lý ảnh tải lên
            if (detailDto.Img != null && detailDto.Img.Length > 0)
            {
                var fileName = Path.GetFileName(detailDto.Img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await detailDto.Img.CopyToAsync(stream);
                }

                // Giả sử bạn có trường `Image` trong `ChiTietPhieuXuatHang` để lưu đường dẫn ảnh
                newDetail.Image = "/UploadedImages/" + fileName;
            }
            else
            {
                newDetail.Image = ""; // Trường hợp không có ảnh
            }

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
        public async Task<IActionResult> UpdateDetail(int id, [FromForm] ChiTietPhieuXuatHangDto detailDto)
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

            // Xử lý ảnh tải lên mới (nếu có)
            if (detailDto.Img != null && detailDto.Img.Length > 0)
            {
                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(existingDetail.Image))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", Path.GetFileName(existingDetail.Image));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                var fileName = Path.GetFileName(detailDto.Img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await detailDto.Img.CopyToAsync(stream);
                }

                existingDetail.Image = "/UploadedImages/" + fileName;
            }

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

            // Xóa ảnh nếu có
            if (!string.IsNullOrEmpty(detail.Image))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", Path.GetFileName(detail.Image));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.ChiTietPhieuXuatHangs.Remove(detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
