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
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateDetail(int id, [FromForm] ChiTietKiemKeDto detailDto)
		{
			if (detailDto == null)
			{
				return BadRequest("Detail data is null.");
			}

			var existingDetail = await _context.ChiTietKiemKes.FindAsync(id);
			if (existingDetail == null)
			{
				return NotFound("Detail not found.");
			}

			existingDetail.MaSanPham = detailDto.MaSanPham;
			existingDetail.SoLuongTon = detailDto.SoLuongTon;
			existingDetail.SoLuongThucTe = detailDto.SoLuongThucTe;
			existingDetail.TrangThai = detailDto.TrangThai;
			existingDetail.NguyenNhan = detailDto.NguyenNhan;

			// Xử lý ảnh nếu có tải lên ảnh mới
			if (detailDto.Img != null && detailDto.Img.Length > 0)
			{
				// Đường dẫn ảnh cũ
				var oldImagePath = existingDetail.Anh;

				// Lưu ảnh mới
				var fileName = Path.GetFileName(detailDto.Img.FileName);
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await detailDto.Img.CopyToAsync(stream);
				}

				existingDetail.Anh = "/UploadedImages/" + fileName;

				// Xóa ảnh cũ nếu tồn tại
				if (!string.IsNullOrEmpty(oldImagePath))
				{
					var fullOldImagePath = Path.Combine(Directory.GetCurrentDirectory(), oldImagePath.TrimStart('/'));
					if (System.IO.File.Exists(fullOldImagePath))
					{
						System.IO.File.Delete(fullOldImagePath);
					}
				}
			}

			await _context.SaveChangesAsync();

			return NoContent();
		}

		/// <summary>
		/// Xóa một chi tiết kiểm kê theo mã kiểm kê
		/// </summary>
		/// <param name="id">Mã kiểm kê</param>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteDetail(int id)
		{
			var detail = await _context.ChiTietKiemKes.FindAsync(id);
			if (detail == null)
			{
				return NotFound("Detail not found.");
			}

			// Xóa ảnh liên quan nếu có
			if (!string.IsNullOrEmpty(detail.Anh))
			{
				var fullImagePath = Path.Combine(Directory.GetCurrentDirectory(), detail.Anh.TrimStart('/'));
				if (System.IO.File.Exists(fullImagePath))
				{
					System.IO.File.Delete(fullImagePath);
				}
			}

			_context.ChiTietKiemKes.Remove(detail);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
