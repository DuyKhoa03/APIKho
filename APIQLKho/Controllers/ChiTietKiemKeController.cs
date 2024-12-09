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

                // Gán danh sách ảnh vào các trường Anh, Anh2, ...
                newDetail.Anh = imagePaths.ElementAtOrDefault(0);
                newDetail.Anh2 = imagePaths.ElementAtOrDefault(1);
                newDetail.Anh3 = imagePaths.ElementAtOrDefault(2);
                newDetail.Anh4 = imagePaths.ElementAtOrDefault(3);
                newDetail.Anh5 = imagePaths.ElementAtOrDefault(4);
                newDetail.Anh6 = imagePaths.ElementAtOrDefault(5);
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

                existingDetail.Anh = imagePaths.ElementAtOrDefault(0);
                existingDetail.Anh2 = imagePaths.ElementAtOrDefault(1);
                existingDetail.Anh3 = imagePaths.ElementAtOrDefault(2);
                existingDetail.Anh4 = imagePaths.ElementAtOrDefault(3);
                existingDetail.Anh5 = imagePaths.ElementAtOrDefault(4);
                existingDetail.Anh6 = imagePaths.ElementAtOrDefault(5);
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

            // Xóa ảnh liên quan nếu có
            if (!string.IsNullOrEmpty(detail.Anh))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), detail.Anh.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            // Xóa bản ghi
            _context.ChiTietKiemKes.Remove(detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
