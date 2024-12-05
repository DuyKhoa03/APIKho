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
        //lay phieu chi tiet phieu dua vao ma phieu va ma sp
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
            // Cập nhật số lượng trong bảng sản phẩm bằng SQL
            string updateSql = "UPDATE SanPham SET SoLuong = SoLuong + @p0 WHERE MaSanPham = @p1";
            await _context.Database.ExecuteSqlRawAsync(updateSql, detailDto.SoLuong, detailDto.MaSanPham);
            return CreatedAtAction(nameof(GetById), new { id = newDetail.MaPhieuNhapHang }, newDetail);
        }

        /// <summary>
        /// Cập nhật một chi tiết phiếu nhập hàng theo mã phiếu
        /// </summary>
        /// <param name="id">Mã phiếu nhập hàng</param>
        /// <param name="detailDto">Dữ liệu chi tiết phiếu nhập hàng cần cập nhật</param>
        [HttpPut("{id}/{productId}")]
        public async Task<IActionResult> UpdateDetail(int id, int productId, [FromForm] ChiTietPhieuNhapHangDto detailDto)
        {
            if (detailDto == null)
            {
                return BadRequest("Detail data is null.");
            }

            // Tìm bản ghi hiện tại
            var existingDetail = await _context.ChiTietPhieuNhapHangs
                .Where(d => d.MaPhieuNhapHang == id && d.MaSanPham == productId)
                .FirstOrDefaultAsync();

            if (existingDetail == null)
            {
                return NotFound("Detail not found.");
            }
            
            // Nếu cần thay đổi MaSanPham, phải xóa bản ghi cũ và thêm bản ghi mới
            if (existingDetail.MaSanPham != detailDto.MaSanPham)
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

                // Xóa bản ghi cũ
                _context.ChiTietPhieuNhapHangs.Remove(existingDetail);
                await _context.SaveChangesAsync();

                // Tạo bản ghi mới
                var newDetail = new ChiTietPhieuNhapHang
                {
                    MaPhieuNhapHang = id,
                    MaSanPham = detailDto.MaSanPham,
                    SoLuong = detailDto.SoLuong,
                    DonGiaNhap = detailDto.DonGiaNhap,
                    TrangThai = detailDto.TrangThai,
                };

                // Lưu ảnh mới nếu có
                if (detailDto.Img != null && detailDto.Img.Length > 0)
                {
                    var fileName = Path.GetFileName(detailDto.Img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await detailDto.Img.CopyToAsync(stream);
                    }

                    newDetail.Image = "/UploadedImages/" + fileName;
                }
                // Tính toán sự thay đổi số lượng
                var quantityDifference = detailDto.SoLuong - existingDetail.SoLuong;
                _context.ChiTietPhieuNhapHangs.Add(newDetail);
                await _context.SaveChangesAsync();
                // Cập nhật số lượng trong bảng sản phẩm
                string updateSql = "UPDATE SanPham SET SoLuong = SoLuong + @p0 WHERE MaSanPham = @p1";
                await _context.Database.ExecuteSqlRawAsync(updateSql, quantityDifference, productId);
                return NoContent();
            }

            // Nếu không cần thay đổi MaSanPham, tiếp tục cập nhật các thuộc tính khác
            existingDetail.SoLuong = detailDto.SoLuong;
            existingDetail.DonGiaNhap = detailDto.DonGiaNhap;
            existingDetail.TrangThai = detailDto.TrangThai;

            // Xử lý ảnh nếu có tải lên mới
            if (detailDto.Img != null && detailDto.Img.Length > 0)
            {
                // Xóa ảnh cũ nếu tồn tại
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
        [HttpDelete("{maPhieuNhap}/{maSanPham}")]
        public async Task<IActionResult> DeleteDetail(int maPhieuNhap, int maSanPham)
        {
            // Tìm chi tiết phiếu nhập hàng dựa trên mã phiếu nhập và mã sản phẩm
            var detail = await _context.ChiTietPhieuNhapHangs
                                       .FirstOrDefaultAsync(p => p.MaPhieuNhapHang == maPhieuNhap && p.MaSanPham == maSanPham);

            // Kiểm tra nếu không tìm thấy chi tiết phiếu
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

            // Xóa chi tiết phiếu nhập
            _context.ChiTietPhieuNhapHangs.Remove(detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
