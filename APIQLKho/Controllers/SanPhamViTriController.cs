using APIQLKho.Dtos;
using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class SanPhamViTriController : ControllerBase
	{
		private readonly QlkhohangContext _context;

		public SanPhamViTriController(QlkhohangContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Lấy danh sách tất cả các sản phẩm tại các vị trí.
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<SanPhamViTriDto>>> Get()
		{
			var productLocations = await _context.SanPhamViTris
												 .Include(spv => spv.MaViTriNavigation)
												 .Include(spv => spv.MaSanPhamNavigation)
												 .Select(spv => new SanPhamViTriDto
												 {
													 MaViTri = spv.MaViTri,
													 MaSanPham = spv.MaSanPham,
													 SoLuong = spv.SoLuong,
													 KhuVuc = spv.MaViTriNavigation.KhuVuc,
													 Tang = spv.MaViTriNavigation.Tang,
													 Ke = spv.MaViTriNavigation.Ke,
													 Mota = spv.MaViTriNavigation.Mota
												 })
												 .ToListAsync();

			return Ok(productLocations);
		}

		/// <summary>
		/// Lấy thông tin chi tiết sản phẩm tại một vị trí dựa vào mã vị trí.
		/// </summary>
		[HttpGet("{maViTri}")]
		public async Task<ActionResult<IEnumerable<SanPhamViTriDto>>> GetByViTri(int maViTri)
		{
			var productLocations = await _context.SanPhamViTris
												 .Where(spv => spv.MaViTri == maViTri)
												 .Include(spv => spv.MaViTriNavigation)
												 .Include(spv => spv.MaSanPhamNavigation)
												 .Select(spv => new SanPhamViTriDto
												 {
													 MaViTri = spv.MaViTri,
													 MaSanPham = spv.MaSanPham,
													 SoLuong = spv.SoLuong,
													 KhuVuc = spv.MaViTriNavigation.KhuVuc,
													 Tang = spv.MaViTriNavigation.Tang,
													 Ke = spv.MaViTriNavigation.Ke,
													 Mota = spv.MaViTriNavigation.Mota
												 })
												 .ToListAsync();

			if (!productLocations.Any())
			{
				return NotFound("No products found at the specified location.");
			}

			return Ok(productLocations);
		}
        /// <summary>
        /// Lấy chi tiết sản phẩm tại vị trí dựa trên mã vị trí và mã sản phẩm.
        /// </summary>
        /// <param name="maViTri">Mã vị trí</param>
        /// <param name="maSanPham">Mã sản phẩm</param>
        [HttpGet("{maViTri}/{maSanPham}")]
        public async Task<ActionResult<SanPhamViTriDto>> GetDetail(int maViTri, int maSanPham)
        {
            var detail = await _context.SanPhamViTris
                                       .Include(spv => spv.MaViTriNavigation)
                                       .Include(spv => spv.MaSanPhamNavigation)
                                       .Where(spv => spv.MaViTri == maViTri && spv.MaSanPham == maSanPham)
                                       .Select(spv => new SanPhamViTriDto
                                       {
                                           MaViTri = spv.MaViTri,
                                           MaSanPham = spv.MaSanPham,
                                           SoLuong = spv.SoLuong,
                                           KhuVuc = spv.MaViTriNavigation.KhuVuc,
                                           Tang = spv.MaViTriNavigation.Tang,
                                           Ke = spv.MaViTriNavigation.Ke,
                                           Mota = spv.MaViTriNavigation.Mota
                                       })
                                       .FirstOrDefaultAsync();

            if (detail == null)
            {
                return NotFound("Product-location detail not found.");
            }

            return Ok(detail);
        }

        /// <summary>
        /// Tạo mới thông tin sản phẩm tại vị trí.
        /// </summary>
        [HttpPost]
		public async Task<ActionResult<SanPhamViTriDto>> Create(SanPhamViTriDto sanPhamViTriDto)
		{
			if (sanPhamViTriDto == null)
			{
				return BadRequest("Product-location data is null.");
			}

			var newProductLocation = new SanPhamViTri
			{
				MaViTri = sanPhamViTriDto.MaViTri,
				MaSanPham = sanPhamViTriDto.MaSanPham,
				SoLuong = sanPhamViTriDto.SoLuong
			};

			_context.SanPhamViTris.Add(newProductLocation);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetByViTri), new { maViTri = newProductLocation.MaViTri }, sanPhamViTriDto);
		}

		/// <summary>
		/// Cập nhật thông tin sản phẩm tại vị trí.
		/// </summary>
		[HttpPut("{maViTri}/{maSanPham}")]
		public async Task<IActionResult> Update(int maViTri, int maSanPham, SanPhamViTriDto sanPhamViTriDto)
		{
			if (sanPhamViTriDto == null)
			{
				return BadRequest("Product-location data is null.");
			}

			var existingProductLocation = await _context.SanPhamViTris
														.FirstOrDefaultAsync(spv => spv.MaViTri == maViTri && spv.MaSanPham == maSanPham);
			if (existingProductLocation == null)
			{
				return NotFound("Product-location not found.");
			}

			existingProductLocation.SoLuong = sanPhamViTriDto.SoLuong;

			await _context.SaveChangesAsync();

			return NoContent();
		}

		/// <summary>
		/// Xóa thông tin sản phẩm tại vị trí.
		/// </summary>
		[HttpDelete("{maViTri}/{maSanPham}")]
		public async Task<IActionResult> Delete(int maViTri, int maSanPham)
		{
			var productLocation = await _context.SanPhamViTris
												.FirstOrDefaultAsync(spv => spv.MaViTri == maViTri && spv.MaSanPham == maSanPham);

			if (productLocation == null)
			{
				return NotFound("Product-location not found.");
			}

			_context.SanPhamViTris.Remove(productLocation);

			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
