using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class NhaCungCapController : ControllerBase
    {
        private readonly ILogger<NhaCungCapController> _logger;
        private readonly QlkhohangContext _context;

        public NhaCungCapController(ILogger<NhaCungCapController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các nhà cung cấp.
        /// </summary>
        /// <returns>Danh sách các nhà cung cấp, bao gồm thông tin về các phiếu nhập hàng liên quan.</returns>
        // GET: api/nhacungcap
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhaCungCap>>> Get()
        {
            var suppliers = await _context.NhaCungCaps
                                          .Include(ncc => ncc.PhieuNhapHangs) // Bao gồm thông tin phiếu nhập hàng
                                          .ToListAsync();
            return Ok(suppliers);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một nhà cung cấp dựa vào ID.
        /// </summary>
        /// <param name="id">ID của nhà cung cấp cần lấy thông tin.</param>
        /// <returns>Thông tin chi tiết của nhà cung cấp nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        // GET: api/nhacungcap/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<NhaCungCap>> GetById(int id)
        {
            var supplier = await _context.NhaCungCaps
                                         .Include(ncc => ncc.PhieuNhapHangs)
                                         .FirstOrDefaultAsync(ncc => ncc.MaNhaCungCap == id);

            if (supplier == null)
            {
                return NotFound("Supplier not found.");
            }

            return Ok(supplier);
        }

        /// <summary>
        /// Thêm mới một nhà cung cấp vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="newSupplier">Thông tin của nhà cung cấp mới cần thêm.</param>
        /// <returns>Nhà cung cấp vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
        // POST: api/nhacungcap
        [HttpPost]
        public async Task<ActionResult<NhaCungCap>> CreateSupplier(NhaCungCap newSupplier)
        {
            if (newSupplier == null)
            {
                return BadRequest("Supplier data is null.");
            }

            _context.NhaCungCaps.Add(newSupplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newSupplier.MaNhaCungCap }, newSupplier);
        }

        /// <summary>
        /// Cập nhật thông tin của một nhà cung cấp dựa vào ID.
        /// </summary>
        /// <param name="id">ID của nhà cung cấp cần cập nhật.</param>
        /// <param name="updatedSupplier">Thông tin nhà cung cấp cần cập nhật.</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        // PUT: api/nhacungcap/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, NhaCungCap updatedSupplier)
        {
            var existingSupplier = await _context.NhaCungCaps.FindAsync(id);
            if (existingSupplier == null)
            {
                return NotFound("Supplier not found.");
            }

            // Cập nhật các thuộc tính của nhà cung cấp
            existingSupplier.TenNhaCungCap = updatedSupplier.TenNhaCungCap;
            existingSupplier.DiaChi = updatedSupplier.DiaChi;
            existingSupplier.Email = updatedSupplier.Email;
            existingSupplier.Sdt = updatedSupplier.Sdt;
            existingSupplier.Image = updatedSupplier.Image;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.NhaCungCaps.AnyAsync(ncc => ncc.MaNhaCungCap == id))
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

        /// <summary>
        /// Xóa một nhà cung cấp dựa vào ID.
        /// </summary>
        /// <param name="id">ID của nhà cung cấp cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        // DELETE: api/nhacungcap/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.NhaCungCaps.FindAsync(id);
            if (supplier == null)
            {
                return NotFound("Supplier not found.");
            }

            // Xóa nhà cung cấp khỏi cơ sở dữ liệu
            _context.NhaCungCaps.Remove(supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Tìm kiếm nhà cung cấp dựa trên từ khóa trong tên hoặc địa chỉ.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (trong tên hoặc địa chỉ của nhà cung cấp).</param>
        /// <returns>Danh sách nhà cung cấp có chứa từ khóa trong tên hoặc địa chỉ.</returns>
        // GET: api/nhacungcap/search/{keyword}
        [HttpGet("{keyword}")]
        public async Task<ActionResult<IEnumerable<NhaCungCap>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var searchResults = await _context.NhaCungCaps
                                              .Where(ncc => ncc.TenNhaCungCap.Contains(keyword) || ncc.DiaChi.Contains(keyword))
                                              .ToListAsync();

            return Ok(searchResults);
        }

    }
}
