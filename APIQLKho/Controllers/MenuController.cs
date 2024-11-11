using APIQLKho.Dtos;
using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MenuController : ControllerBase
    {
        private readonly ILogger<MenuController> _logger;
        private readonly QlkhohangContext _context;

        public MenuController(ILogger<MenuController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các menu, chỉ bao gồm các menu không bị ẩn.
        /// </summary>
        /// <returns>Danh sách các menu sắp xếp theo thứ tự.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Menu>>> Get()
        {
            var menus = await _context.Menus
                                      .Include(m => m.MaNguoiDungNavigation)
                                      .Where(m => m.Hide == false)
                                      .OrderBy(m => m.Order)
                                      .ToListAsync();
            return Ok(menus);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một menu theo ID.
        /// </summary>
        /// <param name="id">ID của menu cần lấy.</param>
        /// <returns>Thông tin của menu nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Menu>> GetById(int id)
        {
            var menu = await _context.Menus
                                     .Include(m => m.MaNguoiDungNavigation)
                                     .FirstOrDefaultAsync(m => m.MenuId == id && m.Hide == false);

            if (menu == null)
            {
                return NotFound("Menu not found.");
            }

            return Ok(menu);
        }

        /// <summary>
        /// Tạo mới một menu.
        /// </summary>
        /// <param name="newMenuDto">Thông tin menu mới cần tạo.</param>
        /// <returns>Menu vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPost]
        public async Task<ActionResult<Menu>> CreateMenu(MenuDto newMenuDto)
        {
            if (newMenuDto == null)
            {
                return BadRequest("Menu data is null.");
            }

            var newMenu = new Menu
            {
                Name = newMenuDto.Name,
                Order = newMenuDto.Order,
                Link = newMenuDto.Link,
                Hide = newMenuDto.Hide,
                MaNguoiDung = newMenuDto.MaNguoiDung
            };

            _context.Menus.Add(newMenu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newMenu.MenuId }, newMenu);
        }

        /// <summary>
        /// Cập nhật thông tin của menu dựa vào ID.
        /// </summary>
        /// <param name="id">ID của menu cần cập nhật.</param>
        /// <param name="updatedMenuDto">Thông tin menu cần cập nhật.</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, MenuDto updatedMenuDto)
        {
            if (updatedMenuDto == null)
            {
                return BadRequest("Menu data is null.");
            }

            var existingMenu = await _context.Menus.FindAsync(id);
            if (existingMenu == null)
            {
                return NotFound("Menu not found.");
            }

            // Cập nhật các thuộc tính
            existingMenu.Name = updatedMenuDto.Name;
            existingMenu.Order = updatedMenuDto.Order;
            existingMenu.Link = updatedMenuDto.Link;
            existingMenu.Hide = updatedMenuDto.Hide;
            existingMenu.MaNguoiDung = updatedMenuDto.MaNguoiDung;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Menus.AnyAsync(m => m.MenuId == id))
                {
                    return NotFound("Menu not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa (ẩn) một menu dựa vào ID.
        /// </summary>
        /// <param name="id">ID của menu cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound("Menu not found.");
            }

            // Ẩn menu thay vì xóa vĩnh viễn
            menu.Hide = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Tìm kiếm menu theo tên.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (tên menu).</param>
        /// <returns>Danh sách các menu có tên phù hợp với từ khóa tìm kiếm.</returns>
        // GET: api/menu/search/{keyword}
        [HttpGet("{keyword}")]
        public async Task<ActionResult<IEnumerable<Menu>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var searchResults = await _context.Menus
                                              .Include(m => m.MaNguoiDungNavigation)
                                              .Where(m => m.Name.Contains(keyword) && m.Hide == false)
                                              .OrderBy(m => m.Order)
                                              .ToListAsync();

            return Ok(searchResults);
        }

    }
}
