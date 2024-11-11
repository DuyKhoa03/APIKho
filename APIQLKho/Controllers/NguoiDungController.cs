using APIQLKho.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize] // Chỉ cho phép người dùng đã đăng nhập truy cập vào controller này
    public class NguoiDungController : ControllerBase
    {
        private readonly ILogger<NguoiDungController> _logger;
        private readonly QlkhohangContext _context;

        public NguoiDungController(ILogger<NguoiDungController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
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
                NgayDk = user.NgayDk
            });
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng.
        /// </summary>
        /// <returns>Danh sách người dùng, chỉ bao gồm những người có quyền khác null.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NguoiDung>>> Get()
        {
            var users = await _context.NguoiDungs
                                      .Where(u => u.Quyen != null)
                                      .OrderBy(u => u.TenNguoiDung)
                                      .ToListAsync();
            return Ok(users);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của người dùng theo ID.
        /// </summary>
        /// <param name="id">ID của người dùng.</param>
        /// <returns>Thông tin người dùng nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<NguoiDung>> GetById(int id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);

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
        [Authorize(Policy = "ManagerOnly")] // Chỉ quản lý được phép thêm người dùng
        [HttpPost]
        public async Task<ActionResult<NguoiDung>> CreateUser(NguoiDung newUser)
        {
            newUser.NgayDk = DateTime.Now;
            _context.NguoiDungs.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newUser.MaNguoiDung }, newUser);
        }

        /// <summary>
        /// Cập nhật thông tin của người dùng dựa vào ID.
        /// </summary>
        /// <param name="id">ID của người dùng cần cập nhật.</param>
        /// <param name="updatedUser">Thông tin người dùng cần cập nhật.</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        [Authorize] // Chỉ cho phép người dùng đã đăng nhập
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, NguoiDung updatedUser)
        {
            var currentUser = await GetCurrentUserAsync();

            if (currentUser == null || (currentUser.Quyen != 1 && id != currentUser.MaNguoiDung)) // Chỉ quản lý hoặc bản thân được cập nhật
            {
                return Forbid("Chỉ quản lý hoặc bản thân mới được phép cập nhật thông tin.");
            }

            var existingUser = await _context.NguoiDungs.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            if (currentUser.Quyen == 1)
            {
                // Quản lý được phép cập nhật tất cả các thuộc tính
                existingUser.TenNguoiDung = updatedUser.TenNguoiDung;
                existingUser.Email = updatedUser.Email;
                existingUser.Quyen = updatedUser.Quyen;
            }

            // Cho phép cập nhật các thuộc tính này với cả quản lý và người dùng
            existingUser.TenDangNhap = updatedUser.TenDangNhap;
            existingUser.Sdt = updatedUser.Sdt;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xóa một người dùng dựa vào ID.
        /// </summary>
        /// <param name="id">ID của người dùng cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        [Authorize(Policy = "ManagerOnly")] // Chỉ quản lý được phép xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            _context.NguoiDungs.Remove(user);
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
        public async Task<ActionResult<IEnumerable<NguoiDung>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var searchResults = await _context.NguoiDungs
                                              .Where(u => u.TenDangNhap.Contains(keyword) || u.TenNguoiDung.Contains(keyword))
                                              .ToListAsync();

            return Ok(searchResults);
        }

    }
}
