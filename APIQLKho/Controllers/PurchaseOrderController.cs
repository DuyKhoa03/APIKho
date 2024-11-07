using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly ILogger<PurchaseOrderController> _logger;
        private readonly QlkhohangContext _context;

        public PurchaseOrderController(ILogger<PurchaseOrderController> logger, QlkhohangContext qlkhohangContext)
        {
            _logger = logger;
            _context = qlkhohangContext;
        }

        // GET: api/purchaseorder
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> Get()
        {
            var purchaseOrders = await _context.PurchaseOrders
                                               .Include(po => po.Supplier)
                                               .Where(po => po.Hide == false)
                                               .OrderBy(po => po.CreatedDate)
                                               .ToListAsync();
            return Ok(purchaseOrders);
        }

        // GET: api/purchaseorder/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrder>> GetById(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders
                                              .Include(po => po.Supplier)
                                              .Include(po => po.Products)
                                              .Include(po => po.EmployeePurchaseOrders)
                                              .FirstOrDefaultAsync(po => po.PurchaseOrderId == id);

            if (purchaseOrder == null || purchaseOrder.Hide == true)
            {
                return NotFound("Purchase order not found.");
            }

            return Ok(purchaseOrder);
        }

        // POST: api/purchaseorder
        [HttpPost]
        public async Task<ActionResult<PurchaseOrder>> CreatePurchaseOrder(PurchaseOrder newPurchaseOrder)
        {
            if (newPurchaseOrder == null)
            {
                return BadRequest("Purchase order data is null.");
            }

            newPurchaseOrder.CreatedDate = DateTime.Now;
            _context.PurchaseOrders.Add(newPurchaseOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newPurchaseOrder.PurchaseOrderId }, newPurchaseOrder);
        }

        // PUT: api/purchaseorder/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchaseOrder(int id, PurchaseOrder updatedPurchaseOrder)
        {
            bool exists = await _context.PurchaseOrders.AnyAsync(c => c.PurchaseOrderId == id);
            if (!exists || id == null)
            {
                return BadRequest("Purchase order not found.");
            }
            var existingPurchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (existingPurchaseOrder == null)
            {
                return NotFound("Purchase order not found.");
            }

            // Update các thuộc tính
            existingPurchaseOrder.SupplierId = updatedPurchaseOrder.SupplierId;
            existingPurchaseOrder.Status = updatedPurchaseOrder.Status;
            existingPurchaseOrder.Image = updatedPurchaseOrder.Image;
            existingPurchaseOrder.UpdateDate = DateTime.Now;
            existingPurchaseOrder.Hide = updatedPurchaseOrder.Hide;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.PurchaseOrders.Any(po => po.PurchaseOrderId == id))
                {
                    return NotFound("Purchase order not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/purchaseorder/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrder(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound("Purchase order not found.");
            }

            purchaseOrder.Hide = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
