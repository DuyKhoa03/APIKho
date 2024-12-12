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
    public class ChiTietKiemKeController : ControllerBase
    {
        private readonly ILogger<ChiTietKiemKeController> _logger;
        private readonly QlkhohangContext _context;
        private readonly Cloudinary _cloudinary;

        public ChiTietKiemKeController(ILogger<ChiTietKiemKeController> logger, QlkhohangContext context, Cloudinary cloudinary)
        {
            _logger = logger;
            _context = context;
            _cloudinary = cloudinary;
        }
        /// <summary>
        /// Lấy danh sách tất cả các chi tiết kiểm kê
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChiTietKiemKeDto>>> Get()
        {
            var details = await _context.ChiTietKiemKes
                                        .Include(ct => ct.MaSanPhamNavigation)
                                        .Include(ct => ct.MaKiemKeNavigation)
                                        .Select(ct => new ChiTietKiemKeDto
                                        {
                                            MaKiemKe = ct.MaKiemKe,
                                            MaSanPham = ct.MaSanPham,
                                            TenSanPham = ct.MaSanPhamNavigation.TenSanPham,
                                            SoLuongTon = ct.SoLuongTon,
                                            SoLuongThucTe = ct.SoLuongThucTe,
                                            TrangThai = ct.TrangThai,
                                            NguyenNhan = ct.NguyenNhan,
                                            Anh = ct.Anh,
                                            Anh2 = ct.Anh2,
                                            Anh3 = ct.Anh3,
                                            Anh4 = ct.Anh4,
                                            Anh5 = ct.Anh5,
                                            Anh6 = ct.Anh6
                                        })
                                        .ToListAsync();

            return Ok(details);
        }


        /// <summary>
        /// Lấy chi tiết kiểm kê theo mã kiểm kê
        /// </summary>
        /// <param name="id">Mã kiểm kê</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ChiTietKiemKeDto>>> GetById(int id)
        {
            var details = await _context.ChiTietKiemKes
                                        .Include(ct => ct.MaSanPhamNavigation)
                                        .Include(ct => ct.MaKiemKeNavigation)
                                        .Where(ct => ct.MaKiemKe == id)
                                        .Select(ct => new ChiTietKiemKeDto
                                        {
                                            MaKiemKe = ct.MaKiemKe,
                                            MaSanPham = ct.MaSanPham,
                                            TenSanPham = ct.MaSanPhamNavigation.TenSanPham,
                                            SoLuongTon = ct.SoLuongTon,
                                            SoLuongThucTe = ct.SoLuongThucTe,
                                            TrangThai = ct.TrangThai,
                                            NguyenNhan = ct.NguyenNhan,
                                            Anh = ct.Anh,
                                            Anh2 = ct.Anh2,
                                            Anh3 = ct.Anh3,
                                            Anh4 = ct.Anh4,
                                            Anh5 = ct.Anh5,
                                            Anh6 = ct.Anh6
                                        })
                                        .ToListAsync();

            if (!details.Any())
            {
                return NotFound("No details found for the specified inventory check ID.");
            }

            return Ok(details);
        }


        [HttpGet("{kiemKeId}/{sanPhamId}")]
        public async Task<ActionResult<ChiTietKiemKeDto>> GetDetail(int kiemKeId, int sanPhamId)
        {
            var detail = await _context.ChiTietKiemKes
                                       .Include(ct => ct.MaSanPhamNavigation)
                                       .Include(ct => ct.MaKiemKeNavigation)
                                       .Where(ct => ct.MaKiemKe == kiemKeId && ct.MaSanPham == sanPhamId)
                                       .Select(ct => new ChiTietKiemKeDto
                                       {
                                           MaKiemKe = ct.MaKiemKe,
                                           MaSanPham = ct.MaSanPham,
                                           TenSanPham = ct.MaSanPhamNavigation.TenSanPham,
                                           SoLuongTon = ct.SoLuongTon,
                                           SoLuongThucTe = ct.SoLuongThucTe,
                                           TrangThai = ct.TrangThai,
                                           NguyenNhan = ct.NguyenNhan,
                                           Anh = ct.Anh,
                                           Anh2 = ct.Anh2,
                                           Anh3 = ct.Anh3,
                                           Anh4 = ct.Anh4,
                                           Anh5 = ct.Anh5,
                                           Anh6 = ct.Anh6
                                       })
                                       .FirstOrDefaultAsync();

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
        public async Task<ActionResult<ChiTietKiemKe>> CreateDetail([FromForm] ChiTietKiemKeDto detailDto)
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
                SoLuongThucTe = detailDto.SoLuongThucTe,
                TrangThai = detailDto.TrangThai,
                NguyenNhan = detailDto.NguyenNhan
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
                        Folder = "inventory-details", // Tên thư mục trên Cloudinary
                        Transformation = new Transformation().Crop("limit").Width(800).Height(800)
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return BadRequest("Failed to upload image to Cloudinary.");
                    }

                    imageUrls.Add(uploadResult.SecureUrl.ToString());
                }

                // Gán URL ảnh vào các trường Anh, Anh2, ...
                newDetail.Anh = imageUrls.ElementAtOrDefault(0);
                newDetail.Anh2 = imageUrls.ElementAtOrDefault(1);
                newDetail.Anh3 = imageUrls.ElementAtOrDefault(2);
                newDetail.Anh4 = imageUrls.ElementAtOrDefault(3);
                newDetail.Anh5 = imageUrls.ElementAtOrDefault(4);
                newDetail.Anh6 = imageUrls.ElementAtOrDefault(5);
            }
            _context.ChiTietKiemKes.Add(newDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newDetail.MaKiemKe }, newDetail);
        }


        /// <summary>
        /// Cập nhật một chi tiết kiểm kê theo mã kiểm kê
        /// </summary>
        /// <param name="id">Mã kiểm kê</param>
        /// <param name="detailDto">Dữ liệu chi tiết kiểm kê cần cập nhật</param>
        [HttpPut("{id}/{productId}")]
        public async Task<IActionResult> UpdateDetail(int id, int productId, [FromForm] ChiTietKiemKeDto detailDto)
        {
            if (detailDto == null)
            {
                return BadRequest("Detail data is null.");
            }

            var existingDetail = await _context.ChiTietKiemKes
                .Where(d => d.MaKiemKe == id && d.MaSanPham == productId)
                .FirstOrDefaultAsync();

            if (existingDetail == null)
            {
                return NotFound("Detail not found.");
            }

            // Cập nhật các thuộc tính
            existingDetail.SoLuongTon = detailDto.SoLuongTon;
            existingDetail.SoLuongThucTe = detailDto.SoLuongThucTe;
            existingDetail.TrangThai = detailDto.TrangThai;
            existingDetail.NguyenNhan = detailDto.NguyenNhan;

            // Xử lý ảnh mới
            if (detailDto.Images != null && detailDto.Images.Any())
            {
                // Xóa ảnh cũ
                //        var oldImages = new List<string>
                //{
                //    existingDetail.Anh,
                //    existingDetail.Anh2,
                //    existingDetail.Anh3,
                //    existingDetail.Anh4,
                //    existingDetail.Anh5,
                //    existingDetail.Anh6
                //};

                //        foreach (var oldImage in oldImages.Where(img => !string.IsNullOrEmpty(img)))
                //        {
                //            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), oldImage.TrimStart('/'));
                //            if (System.IO.File.Exists(oldFilePath))
                //            {
                //                System.IO.File.Delete(oldFilePath);
                //            }
                //        }

                // Lưu ảnh mới
                var imageUrls = new List<string>();
                foreach (var img in detailDto.Images)
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(img.FileName, img.OpenReadStream()),
                        Folder = "inventory-details",
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
                existingDetail.Anh = imageUrls.ElementAtOrDefault(0);
                existingDetail.Anh2 = imageUrls.ElementAtOrDefault(1);
                existingDetail.Anh3 = imageUrls.ElementAtOrDefault(2);
                existingDetail.Anh4 = imageUrls.ElementAtOrDefault(3);
                existingDetail.Anh5 = imageUrls.ElementAtOrDefault(4);
                existingDetail.Anh6 = imageUrls.ElementAtOrDefault(5);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }



        /// <summary>
        /// Xóa một chi tiết kiểm kê theo mã kiểm kê
        /// </summary>
        /// <param name="id">Mã kiểm kê</param>
        [HttpDelete("{kiemKeId}/{sanPhamId}")]
        public async Task<IActionResult> DeleteDetail(int kiemKeId, int sanPhamId)
        {
            var detail = await _context.ChiTietKiemKes
                                       .Where(d => d.MaKiemKe == kiemKeId && d.MaSanPham == sanPhamId)
                                       .FirstOrDefaultAsync();

            if (detail == null)
            {
                return NotFound("Detail not found.");
            }
            // Xóa ảnh liên quan trên Cloudinary nếu có
            var imageUrls = new List<string> { detail.Anh, detail.Anh2, detail.Anh3, detail.Anh4, detail.Anh5, detail.Anh6 };
            foreach (var imageUrl in imageUrls.Where(url => !string.IsNullOrEmpty(url)))
            {
                var publicId = new Uri(imageUrl).Segments.Last().Split('.')[0]; // Trích xuất Public ID từ URL
                var deletionParams = new DeletionParams(publicId);
                await _cloudinary.DestroyAsync(deletionParams);
            }

            // Xóa bản ghi
            _context.ChiTietKiemKes.Remove(detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
