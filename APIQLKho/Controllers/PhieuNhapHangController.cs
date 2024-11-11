using APIQLKho.Dtos;
using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PhieuNhapHangController : ControllerBase
    {
        private readonly ILogger<PhieuNhapHangController> _logger;
        private readonly QlkhohangContext _context;

        public PhieuNhapHangController(ILogger<PhieuNhapHangController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các phiếu nhập hàng.
        /// </summary>
        /// <returns>Một danh sách các phiếu nhập hàng, bao gồm thông tin người dùng, nhà cung cấp và chi tiết phiếu nhập.</returns>
        // GET: api/phieunhaphang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuNhapHang>>> Get()
        {
            var importOrders = await _context.PhieuNhapHangs
                                             .Include(pn => pn.MaNguoiDungNavigation) // Bao gồm thông tin người dùng
                                             .Include(pn => pn.MaNhaCungCapNavigation) // Bao gồm thông tin nhà cung cấp
                                             .Include(pn => pn.ChiTietPhieuNhapHangs) // Bao gồm chi tiết phiếu nhập
                                             .ToListAsync();
            return Ok(importOrders);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một phiếu nhập hàng dựa vào ID.
        /// </summary>
        /// <param name="id">ID của phiếu nhập hàng cần lấy thông tin.</param>
        /// <returns>Thông tin chi tiết của phiếu nhập hàng nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        // GET: api/phieunhaphang/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuNhapHang>> GetById(int id)
        {
            var importOrder = await _context.PhieuNhapHangs
                                            .Include(pn => pn.MaNguoiDungNavigation)
                                            .Include(pn => pn.MaNhaCungCapNavigation)
                                            .Include(pn => pn.ChiTietPhieuNhapHangs)
                                            .FirstOrDefaultAsync(pn => pn.MaPhieuNhapHang == id);

            if (importOrder == null)
            {
                return NotFound("Import order not found.");
            }

            return Ok(importOrder);
        }

        /// <summary>
        /// Thêm mới một phiếu nhập hàng vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="newImportOrderDto">Thông tin phiếu nhập hàng mới cần thêm (dữ liệu từ DTO).</param>
        /// <returns>Phiếu nhập hàng vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
        // POST: api/phieunhaphang
        [HttpPost]
        public async Task<ActionResult<PhieuNhapHang>> CreateImportOrder(PhieuNhapHangDto newImportOrderDto)
        {
            if (newImportOrderDto == null)
            {
                return BadRequest("Import order data is null.");
            }

            var newImportOrder = new PhieuNhapHang
            {
                MaNguoiDung = newImportOrderDto.MaNguoiDung,
                MaNhaCungCap = newImportOrderDto.MaNhaCungCap,
                NgayNhap = newImportOrderDto.NgayNhap,
                PhiVanChuyen = newImportOrderDto.PhiVanChuyen,
                TrangThai = newImportOrderDto.TrangThai
            };

            _context.PhieuNhapHangs.Add(newImportOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newImportOrder.MaPhieuNhapHang }, newImportOrder);
        }

        /// <summary>
        /// Cập nhật thông tin của một phiếu nhập hàng dựa vào ID.
        /// </summary>
        /// <param name="id">ID của phiếu nhập hàng cần cập nhật.</param>
        /// <param name="updatedImportOrderDto">Thông tin phiếu nhập hàng cần cập nhật (dữ liệu từ DTO).</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        // PUT: api/phieunhaphang/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImportOrder(int id, PhieuNhapHangDto updatedImportOrderDto)
        {
            if (updatedImportOrderDto == null)
            {
                return BadRequest("Import order data is null.");
            }

            var existingImportOrder = await _context.PhieuNhapHangs.FindAsync(id);
            if (existingImportOrder == null)
            {
                return NotFound("Import order not found.");
            }

            // Cập nhật các thuộc tính của phiếu nhập hàng
            existingImportOrder.MaNguoiDung = updatedImportOrderDto.MaNguoiDung;
            existingImportOrder.MaNhaCungCap = updatedImportOrderDto.MaNhaCungCap;
            existingImportOrder.NgayNhap = updatedImportOrderDto.NgayNhap;
            existingImportOrder.PhiVanChuyen = updatedImportOrderDto.PhiVanChuyen;
            existingImportOrder.TrangThai = updatedImportOrderDto.TrangThai;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.PhieuNhapHangs.AnyAsync(pn => pn.MaPhieuNhapHang == id))
                {
                    return NotFound("Import order not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa một phiếu nhập hàng dựa vào ID.
        /// </summary>
        /// <param name="id">ID của phiếu nhập hàng cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        // DELETE: api/phieunhaphang/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImportOrder(int id)
        {
            var importOrder = await _context.PhieuNhapHangs.FindAsync(id);
            if (importOrder == null)
            {
                return NotFound("Import order not found.");
            }

            // Xóa phiếu nhập hàng khỏi cơ sở dữ liệu
            _context.PhieuNhapHangs.Remove(importOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Tìm kiếm các phiếu nhập hàng dựa trên từ khóa.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (có thể là tên nhà cung cấp, tên người dùng, hoặc mã phiếu nhập hàng).</param>
        /// <returns>Danh sách các phiếu nhập hàng khớp với từ khóa.</returns>
        // GET: api/phieunhaphang/search/{keyword}
        [HttpGet("{keyword}")]
        public async Task<ActionResult<IEnumerable<PhieuNhapHang>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var searchResults = await _context.PhieuNhapHangs
                                              .Include(pn => pn.MaNguoiDungNavigation)
                                              .Include(pn => pn.MaNhaCungCapNavigation)
                                              .Include(pn => pn.ChiTietPhieuNhapHangs)
                                              .Where(pn => pn.MaPhieuNhapHang.ToString().Contains(keyword) ||
                                                           pn.MaNhaCungCapNavigation.TenNhaCungCap.Contains(keyword) ||
                                                           pn.MaNguoiDungNavigation.TenNguoiDung.Contains(keyword))
                                              .ToListAsync();

            return Ok(searchResults);
        }

    }
}
