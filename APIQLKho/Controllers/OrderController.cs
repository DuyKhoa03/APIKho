using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly QlkhohangContext _context;

        public OrderController(ILogger<OrderController> logger, QlkhohangContext qlkhohangContext)
        {
            _logger = logger;
            _context = qlkhohangContext;
        }

        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> Get()
        {
            var orders = await _context.Orders
                                       .Include(o => o.Customer)
                                       .Where(o => o.Hide == false)
                                       .OrderBy(o => o.CreatedDate)
                                       .ToListAsync();
            return Ok(orders);
        }

        // GET: api/order/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            var order = await _context.Orders
                                      .Include(o => o.Customer)
                                      .Include(o => o.Products)
                                      .Include(o => o.EmployerOrders)
                                      .Include(o => o.Payments)
                                      .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null || order.Hide == true)
            {
                return NotFound("Order not found.");
            }

            return Ok(order);
        }

        // POST: api/order
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order newOrder)
        {
            if (newOrder == null)
            {
                return BadRequest("Order data is null.");
            }

            newOrder.CreatedDate = DateTime.Now;
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newOrder.OrderId }, newOrder);
        }

        // PUT: api/order/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order updatedOrder)
        {
            bool exists = await _context.Orders.AnyAsync(c => c.OrderId == id);
            if (!exists || id == null)
            {
                return BadRequest("Order not found.");
            }
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return NotFound("Order not found.");
            }

            // Update các thuộc tính
            existingOrder.CustomerId = updatedOrder.CustomerId;
            existingOrder.UpdateDate = DateTime.Now;
            existingOrder.Image = updatedOrder.Image;
            existingOrder.Status = updatedOrder.Status;
            existingOrder.TotalAmount = updatedOrder.TotalAmount;
            existingOrder.Discount = updatedOrder.Discount;
            existingOrder.Hide = updatedOrder.Hide;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(o => o.OrderId == id))
                {
                    return NotFound("Order not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/order/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            order.Hide = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}