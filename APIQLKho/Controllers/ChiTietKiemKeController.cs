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
											Anh = ct.Anh
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
											Anh = ct.Anh
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
                                           Anh = ct.Anh
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

			// Xử lý ảnh tải lên (nếu có)
			if (detailDto.Img != null && detailDto.Img.Length > 0)
			{
				var fileName = Path.GetFileName(detailDto.Img.FileName);
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await detailDto.Img.CopyToAsync(stream);
				}

				newDetail.Anh = "/UploadedImages/" + fileName;
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

            // Tìm bản ghi hiện tại
            var existingDetail = await _context.ChiTietKiemKes
                .Where(d => d.MaKiemKe == id && d.MaSanPham == productId)
                .FirstOrDefaultAsync();

            if (existingDetail == null)
            {
                return NotFound("Detail not found.");
            }

            // Nếu cần thay đổi MaSanPham
            if (existingDetail.MaSanPham != detailDto.MaSanPham)
            {
                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(existingDetail.Anh))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", Path.GetFileName(existingDetail.Anh));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Xóa bản ghi cũ
                _context.ChiTietKiemKes.Remove(existingDetail);
                await _context.SaveChangesAsync();

                // Tạo bản ghi mới
                var newDetail = new ChiTietKiemKe
                {
                    MaKiemKe = id,
                    MaSanPham = detailDto.MaSanPham,
                    SoLuongTon = detailDto.SoLuongTon,
                    SoLuongThucTe = detailDto.SoLuongThucTe,
                    TrangThai = detailDto.TrangThai,
                    NguyenNhan = detailDto.NguyenNhan
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

                    newDetail.Anh = "/UploadedImages/" + fileName;
                }

                _context.ChiTietKiemKes.Add(newDetail);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            // Nếu không cần thay đổi MaSanPham, tiếp tục cập nhật các thuộc tính khác
            existingDetail.SoLuongTon = detailDto.SoLuongTon;
            existingDetail.SoLuongThucTe = detailDto.SoLuongThucTe;
            existingDetail.TrangThai = detailDto.TrangThai;
            existingDetail.NguyenNhan = detailDto.NguyenNhan;

            // Xử lý ảnh nếu có tải lên ảnh mới
            if (detailDto.Img != null && detailDto.Img.Length > 0)
            {
                // Xóa ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(existingDetail.Anh))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", Path.GetFileName(existingDetail.Anh));
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

                existingDetail.Anh = "/UploadedImages/" + fileName;
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
