using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PaymentMethodController : ControllerBase
    {
        private readonly ILogger<PaymentMethodController> _logger;
        private readonly QlkhohangContext _context;

        public PaymentMethodController(ILogger<PaymentMethodController> logger, QlkhohangContext qlkhohangContext)
        {
            _logger = logger;
            _context = qlkhohangContext;
        }

        // GET: api/paymentmethod
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethod>>> Get()
        {
            var paymentMethods = await _context.PaymentMethods
                                               .Where(pm => pm.Hide == false)
                                               .OrderBy(pm => pm.Name)
                                               .ToListAsync();
            return Ok(paymentMethods);
        }

        // GET: api/paymentmethod/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentMethod>> GetById(int id)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);

            if (paymentMethod == null || paymentMethod.Hide == true)
            {
                return NotFound("Payment method not found.");
            }

            return Ok(paymentMethod);
        }

        // POST: api/paymentmethod
        [HttpPost]
        public async Task<ActionResult<PaymentMethod>> CreatePaymentMethod(PaymentMethod newPaymentMethod)
        {
            if (newPaymentMethod == null)
            {
                return BadRequest("Payment method data is null.");
            }

            _context.PaymentMethods.Add(newPaymentMethod);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newPaymentMethod.PaymentMethodId }, newPaymentMethod);
        }

        // PUT: api/paymentmethod/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePaymentMethod(int id, PaymentMethod updatedPaymentMethod)
        {
            bool exists = await _context.PaymentMethods.AnyAsync(c => c.PaymentMethodId == id);
            if (!exists || id == null)
            {
                return BadRequest("Payment method not found.");
            }
            var existingPaymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (existingPaymentMethod == null)
            {
                return NotFound("Payment method not found.");
            }

            // Update các thuộc tính
            existingPaymentMethod.Name = updatedPaymentMethod.Name;
            existingPaymentMethod.Description = updatedPaymentMethod.Description;
            existingPaymentMethod.Hide = updatedPaymentMethod.Hide;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.PaymentMethods.Any(pm => pm.PaymentMethodId == id))
                {
                    return NotFound("Payment method not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/paymentmethod/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentMethod(int id)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (paymentMethod == null)
            {
                return NotFound("Payment method not found.");
            }

            paymentMethod.Hide = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}