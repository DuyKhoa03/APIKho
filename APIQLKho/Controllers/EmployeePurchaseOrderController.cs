using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EmployeePurchaseOrderController : ControllerBase
    {
        private readonly ILogger<EmployeePurchaseOrderController> _logger;
        private readonly QlkhohangContext _context;

        public EmployeePurchaseOrderController(ILogger<EmployeePurchaseOrderController> logger, QlkhohangContext qlkhohangContext)
        {
            _logger = logger;
            _context = qlkhohangContext;
        }

        // GET: api/employeepurchaseorder
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeePurchaseOrder>>> Get()
        {
            var employeePurchaseOrders = await _context.EmployeePurchaseOrders
                                                       .Include(e => e.Employee)
                                                       .Include(e => e.PurchaseOrder)
                                                       .ToListAsync();
            return Ok(employeePurchaseOrders);
        }

        // GET: api/employeepurchaseorder/{employeeId}/{purchaseOrderId}
        [HttpGet("{employeeId}/{purchaseOrderId}")]
        public async Task<ActionResult<EmployeePurchaseOrder>> GetById(int employeeId, int purchaseOrderId)
        {
            var employeePurchaseOrder = await _context.EmployeePurchaseOrders
                                                      .Include(e => e.Employee)
                                                      .Include(e => e.PurchaseOrder)
                                                      .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.PurchaseOrderId == purchaseOrderId);

            if (employeePurchaseOrder == null)
            {
                return NotFound("Employee-PurchaseOrder relation not found.");
            }

            return Ok(employeePurchaseOrder);
        }

        // POST: api/employeepurchaseorder
        [HttpPost]
        public async Task<ActionResult<EmployeePurchaseOrder>> CreateEmployeePurchaseOrder(EmployeePurchaseOrder newEmployeePurchaseOrder)
        {
            if (newEmployeePurchaseOrder == null)
            {
                return BadRequest("EmployeePurchaseOrder data is null.");
            }

            _context.EmployeePurchaseOrders.Add(newEmployeePurchaseOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { employeeId = newEmployeePurchaseOrder.EmployeeId, purchaseOrderId = newEmployeePurchaseOrder.PurchaseOrderId }, newEmployeePurchaseOrder);
        }

        // PUT: api/employeepurchaseorder/{employeeId}/{purchaseOrderId}
        [HttpPut("{employeeId}/{purchaseOrderId}")]
        public async Task<IActionResult> UpdateEmployeePurchaseOrder(int employeeId, int purchaseOrderId, EmployeePurchaseOrder updatedEmployeePurchaseOrder)
        {
            if (employeeId != updatedEmployeePurchaseOrder.EmployeeId || purchaseOrderId != updatedEmployeePurchaseOrder.PurchaseOrderId)
            {
                return BadRequest("EmployeePurchaseOrder ID mismatch.");
            }

            var existingEmployeePurchaseOrder = await _context.EmployeePurchaseOrders.FindAsync(employeeId, purchaseOrderId);
            if (existingEmployeePurchaseOrder == null)
            {
                return NotFound("Employee-PurchaseOrder relation not found.");
            }

            // Update các thuộc tính
            existingEmployeePurchaseOrder.Role = updatedEmployeePurchaseOrder.Role;
            existingEmployeePurchaseOrder.DateAssigned = updatedEmployeePurchaseOrder.DateAssigned;
            existingEmployeePurchaseOrder.ActionType = updatedEmployeePurchaseOrder.ActionType;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.EmployeePurchaseOrders.Any(e => e.EmployeeId == employeeId && e.PurchaseOrderId == purchaseOrderId))
                {
                    return NotFound("Employee-PurchaseOrder relation not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/employeepurchaseorder/{employeeId}/{purchaseOrderId}
        [HttpDelete("{employeeId}/{purchaseOrderId}")]
        public async Task<IActionResult> DeleteEmployeePurchaseOrder(int employeeId, int purchaseOrderId)
        {
            var employeePurchaseOrder = await _context.EmployeePurchaseOrders.FindAsync(employeeId, purchaseOrderId);
            if (employeePurchaseOrder == null)
            {
                return NotFound("Employee-PurchaseOrder relation not found.");
            }

            _context.EmployeePurchaseOrders.Remove(employeePurchaseOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
