using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StorageLocationController : ControllerBase
    {
        private readonly ILogger<StorageLocationController> _logger;
        private readonly QlkhohangContext _context;

        public StorageLocationController(ILogger<StorageLocationController> logger, QlkhohangContext qlkhohangContext)
        {
            _logger = logger;
            _context = qlkhohangContext;
        }

        // GET: api/storagelocation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StorageLocation>>> Get()
        {
            var storageLocations = await _context.StorageLocations
                                                 .Include(sl => sl.Products)
                                                 .OrderBy(sl => sl.Name)
                                                 .ToListAsync();
            return Ok(storageLocations);
        }

        // GET: api/storagelocation/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<StorageLocation>> GetById(int id)
        {
            var storageLocation = await _context.StorageLocations
                                                 .Include(sl => sl.Products)
                                                 .FirstOrDefaultAsync(sl => sl.LocationId == id);

            if (storageLocation == null)
            {
                return NotFound("Storage location not found.");
            }

            return Ok(storageLocation);
        }

        // POST: api/storagelocation
        [HttpPost]
        public async Task<ActionResult<StorageLocation>> CreateStorageLocation(StorageLocation newStorageLocation)
        {
            if (newStorageLocation == null)
            {
                return BadRequest("Storage location data is null.");
            }

            _context.StorageLocations.Add(newStorageLocation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newStorageLocation.LocationId }, newStorageLocation);
        }

        // PUT: api/storagelocation/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStorageLocation(int id, StorageLocation updatedStorageLocation)
        {
            bool exists = await _context.StorageLocations.AnyAsync(c => c.LocationId == id);
            if (!exists || id == null)
            {
                return BadRequest("Storage location not found.");
            }

            var existingStorageLocation = await _context.StorageLocations.FindAsync(id);
            if (existingStorageLocation == null)
            {
                return NotFound("Storage location not found.");
            }

            // Update các thuộc tính
            existingStorageLocation.Name = updatedStorageLocation.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.StorageLocations.Any(sl => sl.LocationId == id))
                {
                    return NotFound("Storage location not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/storagelocation/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStorageLocation(int id)
        {
            var storageLocation = await _context.StorageLocations.FindAsync(id);
            if (storageLocation == null)
            {
                return NotFound("Storage location not found.");
            }

            _context.StorageLocations.Remove(storageLocation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
