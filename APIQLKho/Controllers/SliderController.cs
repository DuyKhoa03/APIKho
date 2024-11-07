using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SliderController : ControllerBase
    {
        private readonly QlkhohangContext _context;

        public SliderController(QlkhohangContext context)
        {
            _context = context;
        }

        // GET: api/slider
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Slider>>> Get()
        {
            var sliders = await _context.Sliders
                                        .Where(s => s.Hide == false)
                                        .OrderBy(s => s.Order)
                                        .ToListAsync();
            return Ok(sliders);
        }

        // GET: api/slider/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Slider>> GetById(int id)
        {
            var slider = await _context.Sliders
                                       .Where(s => s.SliderId == id && s.Hide == false)
                                       .FirstOrDefaultAsync();
            if (slider == null)
            {
                return NotFound("Slider not found.");
            }

            return Ok(slider);
        }

        // POST: api/slider
        [HttpPost]
        public async Task<ActionResult<Slider>> CreateSlider(Slider newSlider)
        {
            if (newSlider == null)
            {
                return BadRequest("Slider data is null.");
            }

            _context.Sliders.Add(newSlider);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newSlider.SliderId }, newSlider);
        }

        // PUT: api/slider/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSlider(int id, Slider slider)
        {
            bool exists = await _context.Sliders.AnyAsync(c => c.SliderId == id);
            if (!exists || id == null)
            {
                return NotFound("Slider not found.");
            }
            //testkhoa

            var existingSlider = await _context.Sliders.FindAsync(id);
            if (existingSlider == null)
            {
                return NotFound("Slider not found.");
            }

            // Update properties
            existingSlider.Name = slider.Name;
            existingSlider.Image = slider.Image;
            existingSlider.Order = slider.Order;
            existingSlider.Link = slider.Link;
            existingSlider.Hide = slider.Hide;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/slider/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSlider(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
            {
                return NotFound("Slider not found.");
            }

            slider.Hide = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
