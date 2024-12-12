using APIQLKho.Dtos;
using APIQLKho.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        private readonly Cloudinary _cloudinary;
        public ChiTietPhieuXuatHangController(ILogger<ChiTietPhieuXuatHangController> logger, QlkhohangContext context, Cloudinary cloudinary)
        {
            _logger = logger;
            _context = context;
            _cloudinary = cloudinary;
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
        /// Lấy chi tiết phiếu xuất hàng theo mã phiếu
        /// </summary>
        /// <param name="id">Mã phiếu xuất hàng</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ChiTietPhieuXuatHangDto>>> GetById(int id)
        {
            var details = await _context.ChiTietPhieuXuatHangs
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
                return NotFound("No details found for the specified export receipt ID.");
            }

            return Ok(details);
        }

        [HttpGet("{phieuXuatId}/{sanPhamId}")]
		public async Task<ActionResult<ChiTietPhieuXuatHangDto>> GetDetail(int phieuXuatId, int sanPhamId)
		{
			var detail = await _context.ChiTietPhieuXuatHangs
									   .Include(ct => ct.MaSanPhamNavigation)
									   .Include(ct => ct.MaPhieuXuatHangNavigation)
									   .Where(ct => ct.MaPhieuXuatHang == phieuXuatId && ct.MaSanPham == sanPhamId)
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
            // Upload ảnh lên Cloudinary
            if (detailDto.Images != null && detailDto.Images.Any())
            {
                var imageUrls = new List<string>();
                foreach (var img in detailDto.Images)
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(img.FileName, img.OpenReadStream()),
                        Folder = "export-receipt-details", // Thư mục Cloudinary
                        Transformation = new Transformation().Crop("limit").Width(800).Height(800)
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return BadRequest("Failed to upload image to Cloudinary.");
                    }

                    imageUrls.Add(uploadResult.SecureUrl.ToString());
                }

                // Gán URL ảnh vào các trường Image, Image2, ...
                newDetail.Image = imageUrls.ElementAtOrDefault(0);
                newDetail.Image2 = imageUrls.ElementAtOrDefault(1);
                newDetail.Image3 = imageUrls.ElementAtOrDefault(2);
                newDetail.Image4 = imageUrls.ElementAtOrDefault(3);
                newDetail.Image5 = imageUrls.ElementAtOrDefault(4);
                newDetail.Image6 = imageUrls.ElementAtOrDefault(5);
            }
            _context.ChiTietPhieuXuatHangs.Add(newDetail);
            await _context.SaveChangesAsync();

            // Cập nhật số lượng trong bảng sản phẩm
            string updateSql = "UPDATE SanPham SET SoLuong = SoLuong - @p0 WHERE MaSanPham = @p1";
            await _context.Database.ExecuteSqlRawAsync(updateSql, detailDto.SoLuong, detailDto.MaSanPham);

            return CreatedAtAction(nameof(GetById), new { id = newDetail.MaPhieuXuatHang }, newDetail);
        }


        /// <summary>
        /// Cập nhật một chi tiết phiếu xuất hàng theo mã phiếu
        /// </summary>
        /// <param name="id">Mã phiếu xuất hàng</param>
        /// <param name="detailDto">Dữ liệu chi tiết phiếu xuất hàng cần cập nhật</param>
        [HttpPut("{id}/{productId}")]
        public async Task<IActionResult> UpdateDetail(int id, int productId, [FromForm] ChiTietPhieuXuatHangDto detailDto)
        {
            if (detailDto == null)
            {
                return BadRequest("Detail data is null.");
            }

            var existingDetail = await _context.ChiTietPhieuXuatHangs
                                               .Where(d => d.MaPhieuXuatHang == id && d.MaSanPham == productId)
                                               .FirstOrDefaultAsync();
            if (existingDetail == null)
            {
                return NotFound("Detail not found.");
            }

            // Cập nhật các thuộc tính
            var oldQuantity = existingDetail.SoLuong;
            existingDetail.SoLuong = detailDto.SoLuong;
            existingDetail.DonGiaXuat = detailDto.DonGiaXuat;
            existingDetail.TienMat = detailDto.TienMat;
            existingDetail.NganHang = detailDto.NganHang;
            existingDetail.TrangThai = detailDto.TrangThai;
            // Upload ảnh mới lên Cloudinary
            if (detailDto.Images != null && detailDto.Images.Any())
            {
                var imageUrls = new List<string>();
                foreach (var img in detailDto.Images)
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(img.FileName, img.OpenReadStream()),
                        Folder = "export-receipt-details",
                        Transformation = new Transformation().Crop("limit").Width(800).Height(800)
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return BadRequest("Failed to upload image to Cloudinary.");
                    }

                    imageUrls.Add(uploadResult.SecureUrl.ToString());
                }

                // Cập nhật URL ảnh mới
                existingDetail.Image = imageUrls.ElementAtOrDefault(0);
                existingDetail.Image2 = imageUrls.ElementAtOrDefault(1);
                existingDetail.Image3 = imageUrls.ElementAtOrDefault(2);
                existingDetail.Image4 = imageUrls.ElementAtOrDefault(3);
                existingDetail.Image5 = imageUrls.ElementAtOrDefault(4);
                existingDetail.Image6 = imageUrls.ElementAtOrDefault(5);
            }
            await _context.SaveChangesAsync();

            // Cập nhật số lượng trong bảng sản phẩm
            var quantityDifference = detailDto.SoLuong - oldQuantity;
            string updateSql = "UPDATE SanPham SET SoLuong = SoLuong - @p0 WHERE MaSanPham = @p1";
            await _context.Database.ExecuteSqlRawAsync(updateSql, quantityDifference, productId);

            return NoContent();
        }



        /// <summary>
        /// Xóa một chi tiết phiếu xuất hàng theo mã phiếu
        /// </summary>
        /// <param name="id">Mã phiếu xuất hàng</param>
        [HttpDelete("{phieuXuatId}/{sanPhamId}")]
        public async Task<IActionResult> DeleteDetail(int phieuXuatId, int sanPhamId)
        {
            // Tìm bản ghi cần xóa dựa trên MaPhieuXuatHang và MaSanPham
            var detail = await _context.ChiTietPhieuXuatHangs
                                       .Where(p => p.MaPhieuXuatHang == phieuXuatId && p.MaSanPham == sanPhamId)
                                       .FirstOrDefaultAsync();

            if (detail == null)
            {
                return NotFound("Detail not found.");
            }

            // Xóa ảnh liên quan trên Cloudinary
            var imageUrls = new List<string> { detail.Image, detail.Image2, detail.Image3, detail.Image4, detail.Image5, detail.Image6 };
            foreach (var imageUrl in imageUrls.Where(url => !string.IsNullOrEmpty(url)))
            {
                var publicId = new Uri(imageUrl).Segments.Last().Split('.')[0]; // Trích xuất Public ID từ URL
                var deletionParams = new DeletionParams(publicId);
                await _cloudinary.DestroyAsync(deletionParams);
            }

            // Xóa bản ghi trong database
            _context.ChiTietPhieuXuatHangs.Remove(detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
