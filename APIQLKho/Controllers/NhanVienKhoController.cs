using APIQLKho.Dtos;
using APIQLKho.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class NhanVienKhoController : ControllerBase
    {
        private readonly ILogger<NhanVienKhoController> _logger;
        private readonly QlkhohangContext _context;
        private readonly Cloudinary _cloudinary;

        public NhanVienKhoController(ILogger<NhanVienKhoController> logger, QlkhohangContext context, Cloudinary cloudinary)
        {
            _logger = logger;
            _context = context;
            _cloudinary = cloudinary;
        }

        /// <summary>
        /// Lấy danh sách tất cả các nhân viên kho.
        /// </summary>
        /// <returns>Một danh sách các nhân viên kho, bao gồm thông tin kiểm kê liên quan.</returns>
        // GET: api/nhanvienkho
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanVienKhoDto>>> Get()
        {
            var warehouseEmployees = await _context.NhanVienKhos
                                                   .Where(nv => nv.Hide == false || nv.Hide == null)  // Chỉ lấy nhân viên kho không bị ẩn
                                                   .Include(nv => nv.KiemKes) // Bao gồm thông tin kiểm kê
                                                   .Select(nv => new NhanVienKhoDto
                                                   {
                                                       MaNhanVienKho = nv.MaNhanVienKho,
                                                       TenNhanVien = nv.TenNhanVien,
                                                       Email = nv.Email,
                                                       Sdt = nv.Sdt,
                                                       NamSinh = nv.NamSinh,
                                                       Hinhanh = nv.Hinhanh
                                                   })
                                                   .ToListAsync();

            return Ok(warehouseEmployees);
        }


        /// <summary>
        /// Lấy thông tin chi tiết của một nhân viên kho dựa vào ID.
        /// </summary>
        /// <param name="id">ID của nhân viên kho cần lấy thông tin.</param>
        /// <returns>Thông tin chi tiết của nhân viên kho nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        // GET: api/nhanvienkho/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVienKhoDto>> GetById(int id)
        {
            var warehouseEmployee = await _context.NhanVienKhos
                                                  .Where(nv => nv.MaNhanVienKho == id && (nv.Hide == false || nv.Hide == null))  // Chỉ lấy nhân viên kho không bị ẩn
                                                  .Include(nv => nv.KiemKes) // Bao gồm thông tin kiểm kê
                                                  .Select(nv => new NhanVienKhoDto
                                                  {
                                                      MaNhanVienKho = nv.MaNhanVienKho,
                                                      TenNhanVien = nv.TenNhanVien,
                                                      Email = nv.Email,
                                                      Sdt = nv.Sdt,
                                                      NamSinh = nv.NamSinh,
                                                      Hinhanh = nv.Hinhanh
                                                  })
                                                  .FirstOrDefaultAsync();

            if (warehouseEmployee == null)
            {
                return NotFound("Warehouse employee not found.");
            }

            return Ok(warehouseEmployee);
        }


        /// <summary>
        /// Thêm mới một nhân viên kho vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="newEmployee">Thông tin của nhân viên kho mới cần thêm.</param>
        /// <returns>Nhân viên kho vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
        // POST: api/nhanvienkho

        [HttpPost]
        [Route("uploadfile")]
        public async Task<ActionResult<NhanVienKho>> Create([FromForm] NhanVienKhoDto newEmployeeDto)
        {
            if (newEmployeeDto == null)
            {
                return BadRequest("Warehouse employee data is null.");
            }

            var newEmployee = new NhanVienKho
            {
                TenNhanVien = newEmployeeDto.TenNhanVien,
                Email = newEmployeeDto.Email,
                Sdt = newEmployeeDto.Sdt,
                NamSinh = newEmployeeDto.NamSinh,
                Hide = false
            };

            // Upload ảnh lên Cloudinary
            if (newEmployeeDto.Img != null && newEmployeeDto.Img.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(newEmployeeDto.Img.FileName, newEmployeeDto.Img.OpenReadStream()),
                    Folder = "warehouse-employees", // Tên thư mục Cloudinary
                    Transformation = new Transformation().Crop("limit").Width(300).Height(300)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BadRequest("Failed to upload image to Cloudinary.");
                }

                // Gán URL ảnh từ Cloudinary
                newEmployee.Hinhanh = uploadResult.SecureUrl.ToString();
            }

            _context.NhanVienKhos.Add(newEmployee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newEmployee.MaNhanVienKho }, newEmployee);
        }

        /// <summary>
        /// Cập nhật thông tin của một nhân viên kho dựa vào ID.
        /// </summary>
        /// <param name="id">ID của nhân viên kho cần cập nhật.</param>
        /// <param name="updatedEmployeeDto">Thông tin nhân viên kho cần cập nhật từ DTO.</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        /// PUT: api/nhanvienkho/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] NhanVienKhoDto updatedEmployeeDto)
        {
            var existingEmployee = await _context.NhanVienKhos.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound("Warehouse employee not found.");
            }

            // Cập nhật các thuộc tính của nhân viên kho từ DTO
            existingEmployee.TenNhanVien = updatedEmployeeDto.TenNhanVien;
            existingEmployee.Email = updatedEmployeeDto.Email;
            existingEmployee.Sdt = updatedEmployeeDto.Sdt;
            existingEmployee.NamSinh = updatedEmployeeDto.NamSinh;

            // Upload ảnh mới lên Cloudinary
            if (updatedEmployeeDto.Img != null && updatedEmployeeDto.Img.Length > 0)
            {
                // Xóa ảnh cũ trên Cloudinary nếu có
                if (!string.IsNullOrEmpty(existingEmployee.Hinhanh))
                {
                    var publicId = new Uri(existingEmployee.Hinhanh).Segments.Last().Split('.')[0]; // Trích xuất Public ID từ URL
                    var deletionParams = new DeletionParams(publicId);
                    await _cloudinary.DestroyAsync(deletionParams);
                }

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(updatedEmployeeDto.Img.FileName, updatedEmployeeDto.Img.OpenReadStream()),
                    Folder = "warehouse-employees",
                    Transformation = new Transformation().Crop("limit").Width(300).Height(300)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BadRequest("Failed to upload image to Cloudinary.");
                }

                existingEmployee.Hinhanh = uploadResult.SecureUrl.ToString();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.NhanVienKhos.AnyAsync(nv => nv.MaNhanVienKho == id))
                {
                    return NotFound("Warehouse employee not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        /// <summary>
        /// Xóa một nhân viên kho dựa vào ID.
        /// </summary>
        /// <param name="id">ID của nhân viên kho cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        // DELETE: api/nhanvienkho/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.NhanVienKhos.FindAsync(id);
            if (employee == null)
            {
                return NotFound("Warehouse employee not found.");
            }

            // Xóa ảnh trên Cloudinary nếu có
            if (!string.IsNullOrEmpty(employee.Hinhanh))
            {
                var publicId = new Uri(employee.Hinhanh).Segments.Last().Split('.')[0]; // Trích xuất Public ID từ URL
                var deletionParams = new DeletionParams(publicId);
                await _cloudinary.DestroyAsync(deletionParams);
            }
            // Cập nhật trường Hide thành true thay vì xóa nhân viên kho
            employee.Hide = true;
            try
            {
                await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.NhanVienKhos.AnyAsync(nv => nv.MaNhanVienKho == id))
                {
                    return NotFound("Warehouse employee not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Tìm kiếm các nhân viên kho dựa trên từ khóa trong tên hoặc email.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (trong tên hoặc email của nhân viên kho).</param>
        /// <returns>Danh sách các nhân viên kho có chứa từ khóa.</returns>
        // GET: api/nhanvienkho/search/{keyword}
        [HttpGet("{keyword}")]
        public async Task<ActionResult<IEnumerable<NhanVienKho>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var searchResults = await _context.NhanVienKhos
                .Where(nv => nv.Hide == false || nv.Hide == null)
                                              .Include(nv => nv.KiemKes) // Bao gồm thông tin kiểm kê
                                              .Where(nv => nv.TenNhanVien.Contains(keyword) || nv.Email.Contains(keyword))
                                              .ToListAsync();

            return Ok(searchResults);
        }

    }
}
