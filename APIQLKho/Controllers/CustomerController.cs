using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly QlkhohangContext _context;

        public CustomerController(ILogger<CustomerController> logger, QlkhohangContext qlkhohangContext)
        {
            _logger = logger;
            _context = qlkhohangContext;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(Customer newCustomer)
        {
            // Kiểm tra nếu Username hoặc Email đã tồn tại
            if (await _context.Customers.AnyAsync(c => c.Username == newCustomer.Username))
            {
                return BadRequest("Username đã tồn tại.");
            }

            if (await _context.Customers.AnyAsync(c => c.Email == newCustomer.Email))
            {
                return BadRequest("Email đã được sử dụng.");
            }

            // Mã hóa mật khẩu
            newCustomer.Password = BCrypt.Net.BCrypt.HashPassword(newCustomer.Password);
            newCustomer.RegisterDate = DateTime.Now;

            // Thêm khách hàng vào cơ sở dữ liệu
            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newCustomer.CustomerId }, newCustomer);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Kiểm tra xem Username có tồn tại không
            var customer = await _context.Customers.SingleOrDefaultAsync(c => c.Username == username);

            if (customer == null)
            {
                return NotFound("Username không tồn tại.");
            }

            // Kiểm tra mật khẩu
            if (!BCrypt.Net.BCrypt.Verify(password, customer.Password))
            {
                return Unauthorized("Mật khẩu không đúng.");
            }

            // Cập nhật thời gian đăng nhập cuối cùng
            customer.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            // (Tùy chọn) Trả về mã JWT nếu bạn muốn dùng xác thực JWT
            // string token = GenerateJwtToken(customer);
            // return Ok(new { Token = token });

            return Ok("Đăng nhập thành công.");
        }

        // GET: api/customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> Get()
        {
            var customers = await _context.Customers
                                           .Where(c => c.Hide == false)
                                           .OrderBy(c => c.Name)
                                           .ToListAsync();
            return Ok(customers);
        }

        // GET: api/customer/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null || customer.Hide == true)
            {
                return NotFound("Customer not found.");
            }

            return Ok(customer);
        }

        // POST: api/customer
        //[HttpPost]
        //public async Task<ActionResult<Customer>> CreateCustomer(Customer newCustomer)
        //{
        //    if (newCustomer == null)
        //    {
        //        return BadRequest("Customer data is null.");
        //    }

        //    newCustomer.RegisterDate = DateTime.Now;
        //    _context.Customers.Add(newCustomer);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetById), new { id = newCustomer.CustomerId }, newCustomer);
        //}

        // PUT: api/customer/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customer updatedCustomer)
        {
            bool exists = await _context.Customers.AnyAsync(c => c.CustomerId == id);
            if (!exists || id == null)
            {
                return BadRequest("Customer not found.");
            }
            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null)
            {
                return NotFound("Customer not found.");
            }

            // Update các thuộc tính
            existingCustomer.Name = updatedCustomer.Name;
            existingCustomer.Address = updatedCustomer.Address;
            existingCustomer.Email = updatedCustomer.Email;
            existingCustomer.Phone = updatedCustomer.Phone;
            existingCustomer.Username = updatedCustomer.Username;
            existingCustomer.Password = updatedCustomer.Password;
            existingCustomer.UpdateDate = DateTime.Now;
            existingCustomer.Hide = updatedCustomer.Hide;
            existingCustomer.Role = updatedCustomer.Role;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Customers.Any(c => c.CustomerId == id))
                {
                    return NotFound("Customer not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/customer/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            customer.Hide = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
