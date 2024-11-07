using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly QlkhohangContext _context;

        public PaymentController(ILogger<PaymentController> logger, QlkhohangContext qlkhohangContext)
        {
            _logger = logger;
            _context = qlkhohangContext;
        }

        // GET: api/payment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> Get()
        {
            var payments = await _context.Payments
                                         .Include(p => p.Order)
                                         .Include(p => p.PaymentMethod)
                                         .ToListAsync();
            return Ok(payments);
        }

        // GET: api/payment/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetById(int id)
        {
            var payment = await _context.Payments
                                         .Include(p => p.Order)
                                         .Include(p => p.PaymentMethod)
                                         .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
            {
                return NotFound("Payment not found.");
            }

            return Ok(payment);
        }

        // POST: api/payment
        [HttpPost]
        public async Task<ActionResult<Payment>> CreatePayment(Payment newPayment)
        {
            if (newPayment == null)
            {
                return BadRequest("Payment data is null.");
            }

            newPayment.PaymentDate = DateTime.Now;
            _context.Payments.Add(newPayment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newPayment.PaymentId }, newPayment);
        }

        // PUT: api/payment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, Payment updatedPayment)
        {
            bool exists = await _context.Payments.AnyAsync(c => c.PaymentId == id);
            if (!exists || id == null)
            {
                return BadRequest("Payment not found.");
            }
            var existingPayment = await _context.Payments.FindAsync(id);
            if (existingPayment == null)
            {
                return NotFound("Payment not found.");
            }

            // Update các thuộc tính
            existingPayment.Amount = updatedPayment.Amount;
            existingPayment.Status = updatedPayment.Status;
            existingPayment.TransactionId = updatedPayment.TransactionId;
            existingPayment.PaymentMethodId = updatedPayment.PaymentMethodId;
            existingPayment.OrderId = updatedPayment.OrderId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Payments.Any(p => p.PaymentId == id))
                {
                    return NotFound("Payment not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/payment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound("Payment not found.");
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
