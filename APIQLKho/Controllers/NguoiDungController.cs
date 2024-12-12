using APIQLKho.Dtos;
using APIQLKho.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    //[Authorize] // Chỉ cho phép người dùng đã đăng nhập truy cập vào controller này
    public class NguoiDungController : ControllerBase
    {
        private readonly ILogger<NguoiDungController> _logger;
        private readonly QlkhohangContext _context;
        private readonly Cloudinary _cloudinary;

        public NguoiDungController(ILogger<NguoiDungController> logger, QlkhohangContext context, Cloudinary cloudinary)
        {
            _logger = logger;
            _context = context;
            _cloudinary = cloudinary;
        }
        /// <summary>
        /// Xác thực người dùng thông qua tên đăng nhập và mật khẩu.
        /// </summary>
        /// <param name="username">Tên đăng nhập của người dùng.</param>
        /// <param name="password">Mật khẩu của người dùng.</param>
        /// <returns>Thông tin cơ bản của người dùng nếu đăng nhập thành công, hoặc thông báo lỗi nếu không thành công.</returns>
        [AllowAnonymous] // Cho phép truy cập không cần đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.NguoiDungs.SingleOrDefaultAsync(u => u.TenDangNhap == username);

            if (user == null)
            {
                return NotFound("Username không tồn tại.");
            }

            // Kiểm tra mật khẩu
            if (!BCrypt.Net.BCrypt.Verify(password, user.MatKhau))
            {
                return Unauthorized("Mật khẩu không đúng.");
            }

            // Cập nhật thời gian đăng nhập cuối cùng
            user.NgayDk = DateTime.Now;
            await _context.SaveChangesAsync();

            // Trả về thông tin cơ bản của người dùng
            return Ok(new
            {
                MaNguoiDung = user.MaNguoiDung,
                TenNguoiDung = user.TenNguoiDung,
                Quyen = user.Quyen,
                NgayDk = user.NgayDk,
				Anh = user.Anh
            });
        }

		/// <summary>
		/// Lấy danh sách tất cả người dùng.
		/// </summary>
		/// <returns>Danh sách người dùng, chỉ bao gồm những người có quyền khác null.</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<NguoiDungDto>>> Get()
		{
			var users = await _context.NguoiDungs
									  .Where(u => (u.Quyen != null) && (u.Hide == false || u.Hide == null))  // Lấy người dùng không bị ẩn
									  .OrderBy(u => u.TenNguoiDung)
									  .Select(u => new NguoiDungDto
									  {
										  MaNguoiDung = u.MaNguoiDung,
										  TenDangNhap = u.TenDangNhap,
										  MatKhau = null, // Không trả về mật khẩu
										  TenNguoiDung = u.TenNguoiDung,
										  Email = u.Email,
										  Sdt = u.Sdt,
										  Anh = u.Anh,
										  NgayDk = u.NgayDk,
										  Quyen = u.Quyen,
										  Hide = u.Hide
									  })
									  .ToListAsync();

			return Ok(users);
		}



		/// <summary>
		/// Lấy thông tin chi tiết của người dùng theo ID.
		/// </summary>
		/// <param name="id">ID của người dùng.</param>
		/// <returns>Thông tin người dùng nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<NguoiDungDto>> GetById(int id)
		{
			var user = await _context.NguoiDungs
									 .Where(u => u.MaNguoiDung == id && (u.Hide == false || u.Hide == null)) // Lấy người dùng không bị ẩn
									 .Select(u => new NguoiDungDto
									 {
										 MaNguoiDung = u.MaNguoiDung,
										 TenDangNhap = u.TenDangNhap,
										 MatKhau = null, // Không trả về mật khẩu
										 TenNguoiDung = u.TenNguoiDung,
										 Email = u.Email,
										 Sdt = u.Sdt,
										 Anh = u.Anh,
										 NgayDk = u.NgayDk,
										 Quyen = u.Quyen,
										 Hide = u.Hide
									 })
									 .FirstOrDefaultAsync();

			if (user == null)
			{
				return NotFound("User not found.");
			}

			return Ok(user);
		}



		/// <summary>
		/// Tạo mới một người dùng.
		/// </summary>
		/// <param name="newUser">Thông tin người dùng mới cần tạo.</param>
		/// <returns>Người dùng vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
		//[Authorize(Policy = "ManagerOnly")] // Chỉ quản lý được phép thêm người dùng
		[HttpPost]
		public async Task<ActionResult<NguoiDungDto>> CreateUser([FromForm] NguoiDungDto newUserDto)
		{
			if (newUserDto == null)
			{
				return BadRequest("User data is null.");
			}

			var newUser = new NguoiDung
			{
				TenDangNhap = newUserDto.TenDangNhap,
				MatKhau = BCrypt.Net.BCrypt.HashPassword(newUserDto.MatKhau), // Mã hóa mật khẩu
				TenNguoiDung = newUserDto.TenNguoiDung,
				Email = newUserDto.Email,
				Sdt = newUserDto.Sdt,
				NgayDk = DateTime.Now,
				Quyen = newUserDto.Quyen,
				Hide = false
			};

            // Upload ảnh lên Cloudinary
            if (newUserDto.Img != null && newUserDto.Img.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(newUserDto.Img.FileName, newUserDto.Img.OpenReadStream()),
                    Folder = "user-avatars", // Tên thư mục Cloudinary
                    Transformation = new Transformation().Crop("limit").Width(300).Height(300)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BadRequest("Failed to upload image to Cloudinary.");
                }

                // Gán URL ảnh từ Cloudinary
                newUser.Anh = uploadResult.SecureUrl.ToString();
            }

            _context.NguoiDungs.Add(newUser);
			await _context.SaveChangesAsync();

			newUserDto.MaNguoiDung = newUser.MaNguoiDung;

			return CreatedAtAction(nameof(GetById), new { id = newUser.MaNguoiDung }, newUserDto);
		}



		/// <summary>
		/// Cập nhật thông tin của người dùng dựa vào ID.
		/// </summary>
		/// <param name="id">ID của người dùng cần cập nhật.</param>
		/// <param name="updatedUser">Thông tin người dùng cần cập nhật.</param>
		/// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
		//[Authorize] // Chỉ cho phép người dùng đã đăng nhập
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUser(int id, [FromForm] NguoiDungDto updatedUserDto)
		{
			if (updatedUserDto == null)
			{
				return BadRequest("User data is null.");
			}

			var existingUser = await _context.NguoiDungs.FindAsync(id);
			if (existingUser == null)
			{
				return NotFound("User not found.");
			}

			existingUser.TenDangNhap = updatedUserDto.TenDangNhap;
			existingUser.TenNguoiDung = updatedUserDto.TenNguoiDung;
			existingUser.Email = updatedUserDto.Email;
			existingUser.Sdt = updatedUserDto.Sdt;
			existingUser.Quyen = updatedUserDto.Quyen;

			if (!string.IsNullOrEmpty(updatedUserDto.MatKhau))
			{
				existingUser.MatKhau = BCrypt.Net.BCrypt.HashPassword(updatedUserDto.MatKhau); // Cập nhật mật khẩu nếu có
			}

            // Upload ảnh mới lên Cloudinary
            if (updatedUserDto.Img != null && updatedUserDto.Img.Length > 0)
            {
                // Xóa ảnh cũ trên Cloudinary nếu có
                if (!string.IsNullOrEmpty(existingUser.Anh))
                {
                    var publicId = new Uri(existingUser.Anh).Segments.Last().Split('.')[0]; // Trích xuất Public ID từ URL
                    var deletionParams = new DeletionParams(publicId);
                    await _cloudinary.DestroyAsync(deletionParams);
                }

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(updatedUserDto.Img.FileName, updatedUserDto.Img.OpenReadStream()),
                    Folder = "user-avatars",
                    Transformation = new Transformation().Crop("limit").Width(300).Height(300)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BadRequest("Failed to upload image to Cloudinary.");
                }

                existingUser.Anh = uploadResult.SecureUrl.ToString();
            }

            await _context.SaveChangesAsync();

			return NoContent();
		}



		/// <summary>
		/// Xóa một người dùng dựa vào ID.
		/// </summary>
		/// <param name="id">ID của người dùng cần xóa.</param>
		/// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
		//[Authorize(Policy = "ManagerOnly")] // Chỉ quản lý được phép xóa
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var user = await _context.NguoiDungs.FindAsync(id);
			if (user == null)
			{
				return NotFound("User not found.");
			}

            // Xóa ảnh trên Cloudinary nếu có
            if (!string.IsNullOrEmpty(user.Anh))
            {
                var publicId = new Uri(user.Anh).Segments.Last().Split('.')[0]; // Trích xuất Public ID từ URL
                var deletionParams = new DeletionParams(publicId);
                await _cloudinary.DestroyAsync(deletionParams);
            }

            // Đánh dấu người dùng là ẩn
            user.Hide = true;

			await _context.SaveChangesAsync();

			return NoContent();
		}




		/// <summary>
		/// Lấy thông tin người dùng hiện tại từ yêu cầu đăng nhập.
		/// </summary>
		/// <returns>Thông tin người dùng nếu tồn tại; nếu không, trả về null.</returns>
		private async Task<NguoiDung?> GetCurrentUserAsync()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "MaNguoiDung");

            if (userIdClaim == null)
            {
                return null;
            }

            if (int.TryParse(userIdClaim.Value, out int userId))
            {
                return await _context.NguoiDungs.FindAsync(userId);
            }

            return null;
        }
		/// <summary>
		/// Tìm kiếm người dùng dựa trên từ khóa trong tên đăng nhập hoặc tên người dùng.
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (trong tên đăng nhập hoặc tên người dùng).</param>
		/// <returns>Danh sách người dùng có chứa từ khóa trong tên đăng nhập hoặc tên người dùng.</returns>
		// GET: api/nguoidung/search/{keyword}
		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<NguoiDungDto>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Keyword cannot be empty.");
			}

			var searchResults = await _context.NguoiDungs
											  .Where(u => (u.TenDangNhap.Contains(keyword) || u.TenNguoiDung.Contains(keyword)) && (u.Hide == false || u.Hide == null))
											  .Select(u => new NguoiDungDto
											  {
												  MaNguoiDung = u.MaNguoiDung,
												  TenDangNhap = u.TenDangNhap,
												  MatKhau = null, // Không trả về mật khẩu
												  TenNguoiDung = u.TenNguoiDung,
												  Email = u.Email,
												  Sdt = u.Sdt,
												  Anh = u.Anh,
												  NgayDk = u.NgayDk,
												  Quyen = u.Quyen,
												  Hide = u.Hide
											  })
											  .ToListAsync();

			return Ok(searchResults);
		}


	}
}
