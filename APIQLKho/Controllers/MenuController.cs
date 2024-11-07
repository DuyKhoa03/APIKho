using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MenuController : ControllerBase
    {
        private readonly QlkhohangContext _context;

        public MenuController(QlkhohangContext context)
        {
            _context = context;
        }

        // GET: api/menu
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Menu>>> Get()
        {
            var menus = await _context.Menus
                                      .Where(m => m.Hide == false)
                                      .OrderBy(m => m.Order)
                                      .ToListAsync();
            return Ok(menus);
        }

        // GET: api/menu/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Menu>> GetById(int id)
        {
            var menu = await _context.Menus
                                     .Where(m => m.MenuId == id && m.Hide == false)
                                     .FirstOrDefaultAsync();
            if (menu == null)
            {
                return NotFound("Menu not found.");
            }

            return Ok(menu);
        }

        // POST: api/menu
        [HttpPost]
        public async Task<ActionResult<Menu>> CreateMenu(Menu newMenu)
        {
            if (newMenu == null)
            {
                return BadRequest("Menu data is null.");
            }

            _context.Menus.Add(newMenu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newMenu.MenuId }, newMenu);
        }

        // PUT: api/menu/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, Menu menu)
        {
            
            bool exists = await _context.Menus.AnyAsync(c => c.MenuId == id);
            if (!exists || id == null)
            {
                return NotFound("Menu not found.");
            }
            var existingMenu = await _context.Menus.FindAsync(id);
            if (existingMenu == null)
            {
                return NotFound("Menu not found.");
            }

            // Update properties
            existingMenu.Name = menu.Name;
            existingMenu.Order = menu.Order;
            existingMenu.Link = menu.Link;
            existingMenu.Hide = menu.Hide;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/menu/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound("Menu not found.");
            }

            menu.Hide = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
