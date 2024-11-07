using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EmployerOrderController : ControllerBase
    {
        private readonly ILogger<EmployerOrderController> _logger;
        private readonly QlkhohangContext _context;

        public EmployerOrderController(ILogger<EmployerOrderController> logger, QlkhohangContext qlkhohangContext)
        {
            _logger = logger;
            _context = qlkhohangContext;
        }

        // GET: api/employerorder
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployerOrder>>> Get()
        {
            var employerOrders = await _context.EmployerOrders
                                               .Include(e => e.Employee)
                                               .Include(o => o.Order)
                                               .ToListAsync();
            return Ok(employerOrders);
        }

        // GET: api/employerorder/{employeeId}/{orderId}
        [HttpGet("{employeeId}/{orderId}")]
        public async Task<ActionResult<EmployerOrder>> GetById(int employeeId, int orderId)
        {
            var employerOrder = await _context.EmployerOrders
                                              .Include(e => e.Employee)
                                              .Include(o => o.Order)
                                              .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.OrderId == orderId);

            if (employerOrder == null)
            {
                return NotFound("Employer-Order relation not found.");
            }

            return Ok(employerOrder);
        }

        // POST: api/employerorder
        [HttpPost]
        public async Task<ActionResult<EmployerOrder>> CreateEmployerOrder(EmployerOrder newEmployerOrder)
        {
            if (newEmployerOrder == null)
            {
                return BadRequest("EmployerOrder data is null.");
            }

            _context.EmployerOrders.Add(newEmployerOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { employeeId = newEmployerOrder.EmployeeId, orderId = newEmployerOrder.OrderId }, newEmployerOrder);
        }

        // PUT: api/employerorder/{employeeId}/{orderId}
        [HttpPut("{employeeId}/{orderId}")]
        public async Task<IActionResult> UpdateEmployerOrder(int employeeId, int orderId, EmployerOrder updatedEmployerOrder)
        {
            if (employeeId != updatedEmployerOrder.EmployeeId || orderId != updatedEmployerOrder.OrderId)
            {
                return BadRequest("EmployerOrder ID mismatch.");
            }

            var existingEmployerOrder = await _context.EmployerOrders.FindAsync(employeeId, orderId);
            if (existingEmployerOrder == null)
            {
                return NotFound("Employer-Order relation not found.");
            }

            // Update các thuộc tính
            existingEmployerOrder.Role = updatedEmployerOrder.Role;
            existingEmployerOrder.DateAssigned = updatedEmployerOrder.DateAssigned;
            existingEmployerOrder.ActionType = updatedEmployerOrder.ActionType;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.EmployerOrders.Any(e => e.EmployeeId == employeeId && e.OrderId == orderId))
                {
                    return NotFound("Employer-Order relation not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/employerorder/{employeeId}/{orderId}
        [HttpDelete("{employeeId}/{orderId}")]
        public async Task<IActionResult> DeleteEmployerOrder(int employeeId, int orderId)
        {
            var employerOrder = await _context.EmployerOrders.FindAsync(employeeId, orderId);
            if (employerOrder == null)
            {
                return NotFound("Employer-Order relation not found.");
            }

            _context.EmployerOrders.Remove(employerOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
