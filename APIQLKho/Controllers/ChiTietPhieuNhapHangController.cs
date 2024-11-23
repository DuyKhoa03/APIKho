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
        public async Task<ActionResult<IEnumerable<ChiTietPhieuNhapHangDto>>> Get()
        {
            var details = await _context.ChiTietPhieuNhapHangs
                                        .Include(ct => ct.MaSanPhamNavigation)
                                        .Include(ct => ct.MaPhieuNhapHangNavigation)
                                        .Select(ct => new ChiTietPhieuNhapHangDto
                                        {
                                            MaPhieuNhapHang = ct.MaPhieuNhapHang,
                                            MaSanPham = ct.MaSanPham,
                                            TenSanPham = ct.MaSanPhamNavigation.TenSanPham,
                                            SoLuong = ct.SoLuong,
                                            DonGiaNhap = ct.DonGiaNhap,
                                            TrangThai = ct.TrangThai,
                                            Image = ct.Image // Trả về đường dẫn ảnh nếu có
                                        })
                                        .ToListAsync();

            return Ok(details);
        }

		/// <summary>
		/// Lấy chi tiết phiếu nhập hàng theo mã phiếu
		/// </summary>
		/// <param name="id">Mã phiếu nhập hàng</param>
		[HttpGet("{id}")]
		public async Task<ActionResult<IEnumerable<ChiTietPhieuNhapHangDto>>> GetById(int id)
		{
			var details = await _context.ChiTietPhieuNhapHangs
										.Include(ct => ct.MaSanPhamNavigation)
										.Include(ct => ct.MaPhieuNhapHangNavigation)
										.Where(ct => ct.MaPhieuNhapHang == id)
										.Select(ct => new ChiTietPhieuNhapHangDto
										{
											MaPhieuNhapHang = ct.MaPhieuNhapHang,
											MaSanPham = ct.MaSanPham,
											TenSanPham = ct.MaSanPhamNavigation.TenSanPham,
											SoLuong = ct.SoLuong,
											DonGiaNhap = ct.DonGiaNhap,
											TrangThai = ct.TrangThai,
											Image = ct.Image
										})
										.ToListAsync();

			if (!details.Any())
			{
				return NotFound("No details found for the specified receipt ID.");
			}

			return Ok(details);
		}
		[HttpGet("{phieuXuatId}/{sanPhamId}")]
		public async Task<ActionResult<ChiTietPhieuNhapHangDto>> GetDetail(int phieuNhapId, int sanPhamId)
		{
			var detail = await _context.ChiTietPhieuNhapHangs
									   .Include(ct => ct.MaSanPhamNavigation)
									   .Include(ct => ct.MaPhieuNhapHangNavigation)
									   .Where(ct => ct.MaPhieuNhapHang == phieuNhapId && ct.MaSanPham == sanPhamId)
									   .Select(ct => new ChiTietPhieuNhapHangDto
									   {
										   MaSanPham = ct.MaSanPham,
										   TenSanPham = ct.MaSanPhamNavigation.TenSanPham,
										   MaPhieuNhapHang = ct.MaPhieuNhapHang,
										   SoLuong = ct.SoLuong,
										   DonGiaNhap = ct.DonGiaNhap,
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
		/// Tạo mới một chi tiết phiếu nhập hàng
		/// </summary>
		/// <param name="detailDto">Dữ liệu chi tiết phiếu nhập hàng cần tạo</param>

		[HttpPost]
        [Route("uploadfile")]
        public async Task<ActionResult<ChiTietPhieuNhapHang>> CreateDetailWithImage([FromForm] ChiTietPhieuNhapHangDto detailDto)
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

                // Giả sử bạn có trường `Image` trong `ChiTietPhieuNhapHang` để lưu đường dẫn ảnh
                newDetail.Image = "/UploadedImages/" + fileName;
            }
            else
            {
                newDetail.Image = ""; // Trường hợp không có ảnh
            }

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
        public async Task<IActionResult> UpdateDetail(int id, [FromForm] ChiTietPhieuNhapHangDto detailDto)
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

            // Xóa ảnh nếu có
            if (!string.IsNullOrEmpty(detail.Image))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", Path.GetFileName(detail.Image));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.ChiTietPhieuNhapHangs.Remove(detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
