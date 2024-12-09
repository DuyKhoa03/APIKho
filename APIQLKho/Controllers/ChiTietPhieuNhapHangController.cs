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
                                            Image = ct.Image,
                                            Image2 = ct.Image2,
                                            Image3 = ct.Image3,
                                            Image4 = ct.Image4,
                                            Image5 = ct.Image5,
                                            Image6 = ct.Image6
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
                                            Image = ct.Image,
                                            Image2 = ct.Image2,
                                            Image3 = ct.Image3,
                                            Image4 = ct.Image4,
                                            Image5 = ct.Image5,
                                            Image6 = ct.Image6
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
										   Image = ct.Image,
                                           Image2 = ct.Image2,
                                           Image3 = ct.Image3,
                                           Image4 = ct.Image4,
                                           Image5 = ct.Image5,
                                           Image6 = ct.Image6
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

            // Xử lý danh sách ảnh tải lên (nếu có)
            if (detailDto.Images != null && detailDto.Images.Any())
            {
                var imagePaths = new List<string>();
                foreach (var img in detailDto.Images)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(stream);
                    }

                    imagePaths.Add("/UploadedImages/" + fileName);
                }

                // Gán đường dẫn ảnh vào các trường Image, Image2, ...
                newDetail.Image = imagePaths.ElementAtOrDefault(0);
                newDetail.Image2 = imagePaths.ElementAtOrDefault(1);
                newDetail.Image3 = imagePaths.ElementAtOrDefault(2);
                newDetail.Image4 = imagePaths.ElementAtOrDefault(3);
                newDetail.Image5 = imagePaths.ElementAtOrDefault(4);
                newDetail.Image6 = imagePaths.ElementAtOrDefault(5);
            }

            _context.ChiTietPhieuNhapHangs.Add(newDetail);
            await _context.SaveChangesAsync();

            // Cập nhật số lượng trong bảng sản phẩm
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

            var existingDetail = await _context.ChiTietPhieuNhapHangs
                                               .Where(d => d.MaPhieuNhapHang == id && d.MaSanPham == productId)
                                               .FirstOrDefaultAsync();
            if (existingDetail == null)
            {
                return NotFound("Detail not found.");
            }

            // Cập nhật các thuộc tính
            var oldQuantity = existingDetail.SoLuong;
            existingDetail.SoLuong = detailDto.SoLuong;
            existingDetail.DonGiaNhap = detailDto.DonGiaNhap;
            existingDetail.TrangThai = detailDto.TrangThai;

            // Xử lý ảnh mới nếu có
            if (detailDto.Images != null && detailDto.Images.Any())
            {
                // Xóa ảnh cũ
        //        var oldImages = new List<string>
        //{
        //    existingDetail.Image,
        //    existingDetail.Image2,
        //    existingDetail.Image3,
        //    existingDetail.Image4,
        //    existingDetail.Image5,
        //    existingDetail.Image6
        //};

        //        foreach (var oldImage in oldImages.Where(i => !string.IsNullOrEmpty(i)))
        //        {
        //            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), oldImage.TrimStart('/'));
        //            if (System.IO.File.Exists(oldFilePath))
        //            {
        //                System.IO.File.Delete(oldFilePath);
        //            }
        //        }

                // Lưu ảnh mới
                var imagePaths = new List<string>();
                foreach (var img in detailDto.Images)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(stream);
                    }

                    imagePaths.Add("/UploadedImages/" + fileName);
                }

                existingDetail.Image = imagePaths.ElementAtOrDefault(0);
                existingDetail.Image2 = imagePaths.ElementAtOrDefault(1);
                existingDetail.Image3 = imagePaths.ElementAtOrDefault(2);
                existingDetail.Image4 = imagePaths.ElementAtOrDefault(3);
                existingDetail.Image5 = imagePaths.ElementAtOrDefault(4);
                existingDetail.Image6 = imagePaths.ElementAtOrDefault(5);
            }

            await _context.SaveChangesAsync();

            // Cập nhật số lượng trong bảng sản phẩm
            var quantityDifference = detailDto.SoLuong - oldQuantity;
            string updateSql = "UPDATE SanPham SET SoLuong = SoLuong + @p0 WHERE MaSanPham = @p1";
            await _context.Database.ExecuteSqlRawAsync(updateSql, quantityDifference, productId);

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
