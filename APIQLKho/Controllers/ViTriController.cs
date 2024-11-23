using APIQLKho.Dtos;
using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class ViTriController : ControllerBase
	{
		private readonly QlkhohangContext _context;

		public ViTriController(QlkhohangContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Lấy danh sách tất cả các vị trí.
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<VitriDto>>> Get()
		{
			var locations = await _context.Vitris
										  .Where(vt => vt.Hide == false || vt.Hide == null) // Lấy vị trí không bị ẩn
										  .Select(vt => new VitriDto
										  {
											  MaViTri = vt.MaViTri,
											  KhuVuc = vt.KhuVuc,
											  Tang = vt.Tang,
											  Ke = vt.Ke,
											  Mota = vt.Mota,
											  Hide = vt.Hide
										  })
										  .ToListAsync();

			return Ok(locations);
		}

		/// <summary>
		/// Lấy thông tin chi tiết của một vị trí dựa vào ID.
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<VitriDto>> GetById(int id)
		{
			var location = await _context.Vitris
										 .Where(vt => vt.MaViTri == id && (vt.Hide == false || vt.Hide == null)) // Chỉ lấy vị trí không bị ẩn
										 .Select(vt => new VitriDto
										 {
											 MaViTri = vt.MaViTri,
											 KhuVuc = vt.KhuVuc,
											 Tang = vt.Tang,
											 Ke = vt.Ke,
											 Mota = vt.Mota,
											 Hide = vt.Hide
										 })
										 .FirstOrDefaultAsync();

			if (location == null)
			{
				return NotFound("Location not found.");
			}

			return Ok(location);
		}

		/// <summary>
		/// Tạo mới một vị trí.
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<VitriDto>> Create(VitriDto vitriDto)
		{
			if (vitriDto == null)
			{
				return BadRequest("Location data is null.");
			}

			var newLocation = new Vitri
			{
				KhuVuc = vitriDto.KhuVuc,
				Tang = vitriDto.Tang,
				Ke = vitriDto.Ke,
				Mota = vitriDto.Mota,
				Hide = false // Mặc định không bị ẩn khi tạo mới
			};

			_context.Vitris.Add(newLocation);
			await _context.SaveChangesAsync();

			vitriDto.MaViTri = newLocation.MaViTri;

			return CreatedAtAction(nameof(GetById), new { id = newLocation.MaViTri }, vitriDto);
		}

		/// <summary>
		/// Cập nhật thông tin của một vị trí dựa vào ID.
		/// </summary>
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, VitriDto vitriDto)
		{
			if (vitriDto == null)
			{
				return BadRequest("Location data is null.");
			}

			var existingLocation = await _context.Vitris.FindAsync(id);
			if (existingLocation == null)
			{
				return NotFound("Location not found.");
			}

			existingLocation.KhuVuc = vitriDto.KhuVuc;
			existingLocation.Tang = vitriDto.Tang;
			existingLocation.Ke = vitriDto.Ke;
			existingLocation.Mota = vitriDto.Mota;

			await _context.SaveChangesAsync();

			return NoContent();
		}

		/// <summary>
		/// Xóa (ẩn) một vị trí dựa vào ID.
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var location = await _context.Vitris.FindAsync(id);
			if (location == null)
			{
				return NotFound("Location not found.");
			}

			// Đánh dấu vị trí là ẩn
			location.Hide = true;

			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
