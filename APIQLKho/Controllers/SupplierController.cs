using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SupplierController : ControllerBase
    {
        private readonly ILogger<SupplierController> _logger;
        private readonly QlkhohangContext _context;

        public SupplierController(ILogger<SupplierController> logger, QlkhohangContext qlkhohangContext)
        {
            _logger = logger;
            _context = qlkhohangContext;
        }

        // GET: api/supplier
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> Get()
        {
            var suppliers = await _context.Suppliers
                                           .Where(s => s.Hide == false)
                                           .OrderBy(s => s.Name)
                                           .ToListAsync();
            return Ok(suppliers);
        }

        // GET: api/supplier/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetById(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null || supplier.Hide == true)
            {
                return NotFound("Supplier not found.");
            }

            return Ok(supplier);
        }

        // POST: api/supplier
        [HttpPost]
        public async Task<ActionResult<Supplier>> CreateSupplier(Supplier newSupplier)
        {
            if (newSupplier == null)
            {
                return BadRequest("Supplier data is null.");
            }

            _context.Suppliers.Add(newSupplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newSupplier.SupplierId }, newSupplier);
        }

        // PUT: api/supplier/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, Supplier updatedSupplier)
        {
            bool exists = await _context.Suppliers.AnyAsync(c => c.SupplierId == id);
           
            if (!exists || id == null)
            {
                return BadRequest("Supplier not found.");
            }

            var existingSupplier = await _context.Suppliers.FindAsync(id);
            if (existingSupplier == null)
            {
                return NotFound("Supplier not found.");
            }

            // Update các thuộc tính
            existingSupplier.Name = updatedSupplier.Name;
            existingSupplier.Address = updatedSupplier.Address;
            existingSupplier.Email = updatedSupplier.Email;
            existingSupplier.Phone = updatedSupplier.Phone;
            existingSupplier.UpdateDate = updatedSupplier.UpdateDate;
            existingSupplier.Hide = updatedSupplier.Hide;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Suppliers.Any(e => e.SupplierId == id))
                {
                    return NotFound("Supplier not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/supplier/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound("Supplier not found.");
            }

            supplier.Hide = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}