using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly QlkhohangContext _context;

        public EmployeeController(ILogger<EmployeeController> logger, QlkhohangContext qlkhohangContext)
        {
            _logger = logger;
            _context = qlkhohangContext;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Tìm Employee theo Username
            var employee = await _context.Employees.SingleOrDefaultAsync(e => e.Username == username);

            if (employee == null)
            {
                return NotFound("Username không tồn tại.");
            }

            // Kiểm tra mật khẩu
            if (!BCrypt.Net.BCrypt.Verify(password, employee.Password))
            {
                return Unauthorized("Mật khẩu không đúng.");
            }

            // Cập nhật thời gian đăng nhập cuối cùng
            employee.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            // Trả về thông tin cơ bản của nhân viên
            return Ok(new
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                Role = employee.Role,
                LastLogin = employee.LastLogin
            });
        }

        // GET: api/employee
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> Get()
        {
            var employees = await _context.Employees
                                           .Where(e => e.Hide == false)
                                           .OrderBy(e => e.Name)
                                           .ToListAsync();
            return Ok(employees);
        }

        // GET: api/employee/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null || employee.Hide == true)
            {
                return NotFound("Employee not found.");
            }

            return Ok(employee);
        }

        // POST: api/employee
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee newEmployee)
        {
            // Giả sử bạn có cách để xác định `Employee` hiện tại và kiểm tra quyền của họ
            var currentUser = await GetCurrentEmployeeAsync(); // Giả định có hàm lấy `Employee` hiện tại

            if (currentUser == null || currentUser.Role != 1) // Kiểm tra nếu không phải là quản lý
            {
                return Forbid("Chỉ quản lý mới được phép thêm nhân viên.");
            }

            newEmployee.Registerdate = DateTime.Now;
            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newEmployee.EmployeeId }, newEmployee);
        }

        // PUT: api/employee/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee updatedEmployee)
        {
            var currentUser = await GetCurrentEmployeeAsync();

            if (currentUser == null || (currentUser.Role != 1 && id != currentUser.EmployeeId)) // Chỉ quản lý hoặc bản thân được cập nhật
            {
                return Forbid("Chỉ quản lý hoặc bản thân mới được phép cập nhật thông tin.");
            }

            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound("Employee not found.");
            }

            if (currentUser.Role == 1)
            {
                // Quản lý được phép cập nhật tất cả các thuộc tính
                existingEmployee.Name = updatedEmployee.Name;
                existingEmployee.Position = updatedEmployee.Position;
                existingEmployee.Role = updatedEmployee.Role;
            }

            // Cho phép cập nhật các thuộc tính này với cả quản lý và nhân viên
            existingEmployee.Address = updatedEmployee.Address;
            existingEmployee.Email = updatedEmployee.Email;
            existingEmployee.Phone = updatedEmployee.Phone;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/employee/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var currentUser = await GetCurrentEmployeeAsync();

            if (currentUser == null || currentUser.Role != 1) // Chỉ quản lý mới được phép xóa
            {
                return Forbid("Chỉ quản lý mới được phép xóa nhân viên.");
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            employee.Hide = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private async Task<Employee?> GetCurrentEmployeeAsync()
        {
            // Giả sử Employee ID của người dùng hiện tại được lưu trong yêu cầu
            // Ví dụ: qua User.Claims hoặc HttpContext.Session nếu đã cấu hình
            var employeeIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EmployeeId");

            if (employeeIdClaim == null)
            {
                return null; // Không tìm thấy Employee ID, có thể người dùng chưa đăng nhập
            }

            if (int.TryParse(employeeIdClaim.Value, out int employeeId))
            {
                // Truy vấn cơ sở dữ liệu để lấy thông tin `Employee` hiện tại
                return await _context.Employees.FindAsync(employeeId);
            }

            return null;
        }

    }
}
