using APIQLKho.Dtos;
using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PhieuXuatHangController : ControllerBase
    {
        private readonly ILogger<PhieuXuatHangController> _logger;
        private readonly QlkhohangContext _context;

        public PhieuXuatHangController(ILogger<PhieuXuatHangController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các phiếu xuất hàng.
        /// </summary>
        /// <returns>Một danh sách các phiếu xuất hàng, bao gồm thông tin khách hàng, người dùng và chi tiết phiếu xuất.</returns>
        // GET: api/phieuxuathang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuXuatHang>>> Get()
        {
            var exportOrders = await _context.PhieuXuatHangs
                                             .Include(px => px.MaKhachHangNavigation) // Bao gồm thông tin khách hàng
                                             .Include(px => px.MaNguoiDungNavigation) // Bao gồm thông tin người dùng
                                             .Include(px => px.ChiTietPhieuXuatHangs) // Bao gồm chi tiết phiếu xuất
                                             .ToListAsync();
            return Ok(exportOrders);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một phiếu xuất hàng dựa vào ID.
        /// </summary>
        /// <param name="id">ID của phiếu xuất hàng cần lấy thông tin.</param>
        /// <returns>Thông tin chi tiết của phiếu xuất hàng nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        // GET: api/phieuxuathang/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuXuatHang>> GetById(int id)
        {
            var exportOrder = await _context.PhieuXuatHangs
                                            .Include(px => px.MaKhachHangNavigation)
                                            .Include(px => px.MaNguoiDungNavigation)
                                            .Include(px => px.ChiTietPhieuXuatHangs)
                                            .FirstOrDefaultAsync(px => px.MaPhieuXuatHang == id);

            if (exportOrder == null)
            {
                return NotFound("Export order not found.");
            }

            return Ok(exportOrder);
        }

        /// <summary>
        /// Thêm mới một phiếu xuất hàng vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="newExportOrderDto">Thông tin phiếu xuất hàng mới cần thêm (dữ liệu từ DTO).</param>
        /// <returns>Phiếu xuất hàng vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
        // POST: api/phieuxuathang
        [HttpPost]
        public async Task<ActionResult<PhieuXuatHang>> CreateExportOrder(PhieuXuatHangDto newExportOrderDto)
        {
            if (newExportOrderDto == null)
            {
                return BadRequest("Export order data is null.");
            }

            var newExportOrder = new PhieuXuatHang
            {
                MaNguoiDung = newExportOrderDto.MaNguoiDung,
                MaKhachHang = newExportOrderDto.MaKhachHang,
                NgayXuat = newExportOrderDto.NgayXuat,
                HinhThucThanhToan = newExportOrderDto.HinhThucThanhToan,
                PhiVanChuyen = newExportOrderDto.PhiVanChuyen,
                TrangThai = newExportOrderDto.TrangThai
            };

            _context.PhieuXuatHangs.Add(newExportOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newExportOrder.MaPhieuXuatHang }, newExportOrder);
        }

        /// <summary>
        /// Cập nhật thông tin của một phiếu xuất hàng dựa vào ID.
        /// </summary>
        /// <param name="id">ID của phiếu xuất hàng cần cập nhật.</param>
        /// <param name="updatedExportOrderDto">Thông tin phiếu xuất hàng cần cập nhật (dữ liệu từ DTO).</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        // PUT: api/phieuxuathang/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExportOrder(int id, PhieuXuatHangDto updatedExportOrderDto)
        {
            if (updatedExportOrderDto == null)
            {
                return BadRequest("Export order data is null.");
            }

            var existingExportOrder = await _context.PhieuXuatHangs.FindAsync(id);
            if (existingExportOrder == null)
            {
                return NotFound("Export order not found.");
            }

            // Cập nhật các thuộc tính của phiếu xuất hàng
            existingExportOrder.MaNguoiDung = updatedExportOrderDto.MaNguoiDung;
            existingExportOrder.MaKhachHang = updatedExportOrderDto.MaKhachHang;
            existingExportOrder.NgayXuat = updatedExportOrderDto.NgayXuat;
            existingExportOrder.HinhThucThanhToan = updatedExportOrderDto.HinhThucThanhToan;
            existingExportOrder.PhiVanChuyen = updatedExportOrderDto.PhiVanChuyen;
            existingExportOrder.TrangThai = updatedExportOrderDto.TrangThai;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.PhieuXuatHangs.AnyAsync(px => px.MaPhieuXuatHang == id))
                {
                    return NotFound("Export order not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa một phiếu xuất hàng dựa vào ID.
        /// </summary>
        /// <param name="id">ID của phiếu xuất hàng cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        // DELETE: api/phieuxuathang/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExportOrder(int id)
        {
            var exportOrder = await _context.PhieuXuatHangs.FindAsync(id);
            if (exportOrder == null)
            {
                return NotFound("Export order not found.");
            }

            // Xóa phiếu xuất hàng khỏi cơ sở dữ liệu
            _context.PhieuXuatHangs.Remove(exportOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Tìm kiếm các phiếu xuất hàng dựa trên từ khóa.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (có thể là tên khách hàng, tên người dùng, hoặc mã phiếu xuất hàng).</param>
        /// <returns>Danh sách các phiếu xuất hàng khớp với từ khóa.</returns>
        // GET: api/phieuxuathang/search/{keyword}
        [HttpGet("{keyword}")]
        public async Task<ActionResult<IEnumerable<PhieuXuatHang>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var searchResults = await _context.PhieuXuatHangs
                                              .Include(px => px.MaKhachHangNavigation)
                                              .Include(px => px.MaNguoiDungNavigation)
                                              .Include(px => px.ChiTietPhieuXuatHangs)
                                              .Where(px => px.MaPhieuXuatHang.ToString().Contains(keyword) ||
                                                           px.MaKhachHangNavigation.TenKhachHang.Contains(keyword) ||
                                                           px.MaNguoiDungNavigation.TenNguoiDung.Contains(keyword))
                                              .ToListAsync();

            return Ok(searchResults);
        }

    }
}
