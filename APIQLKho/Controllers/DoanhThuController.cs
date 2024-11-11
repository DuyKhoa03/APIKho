using APIQLKho.Dtos;
using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DoanhThuController : ControllerBase
    {
        private readonly ILogger<DoanhThuController> _logger;
        private readonly QlkhohangContext _context;

        public DoanhThuController(ILogger<DoanhThuController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các bản ghi doanh thu, bao gồm thông tin sản phẩm liên quan.
        /// </summary>
        /// <returns>Danh sách các bản ghi doanh thu.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoanhThu>>> Get()
        {
            // Lấy tất cả các bản ghi doanh thu và bao gồm thông tin sản phẩm liên quan.
            var revenues = await _context.DoanhThus
                .Include(dt => dt.MaSanPhamNavigation) // Bao gồm thông tin sản phẩm
                .ToListAsync();

            return Ok(revenues);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một bản ghi doanh thu dựa vào ID.
        /// </summary>
        /// <param name="id">ID của bản ghi doanh thu cần lấy.</param>
        /// <returns>Thông tin của bản ghi doanh thu nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DoanhThu>> GetById(int id)
        {
            // Tìm và trả về bản ghi doanh thu theo ID
            var revenue = await _context.DoanhThus
                .Include(dt => dt.MaSanPhamNavigation) // Bao gồm thông tin sản phẩm
                .FirstOrDefaultAsync(dt => dt.MaDoanhThu == id);

            if (revenue == null)
            {
                return NotFound("Revenue record not found.");
            }

            return Ok(revenue);
        }

        /// <summary>
        /// Tạo mới một bản ghi doanh thu.
        /// </summary>
        /// <param name="newRevenue">Thông tin của bản ghi doanh thu mới cần tạo.</param>
        /// <returns>Bản ghi doanh thu vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPost]
        public async Task<ActionResult<DoanhThu>> CreateRevenue(DoanhThuDto newRevenueDto)
        {
            if (newRevenueDto == null)
            {
                return BadRequest("Revenue data is null.");
            }
            var newRevenue = new DoanhThu
            {
                MaSanPham = newRevenueDto.MaSanPham,
                TongPhiNhap = newRevenueDto.TongPhiNhap,
                TongPhiXuat = newRevenueDto.TongPhiXuat,
                PhiVanHanh = newRevenueDto.PhiVanHanh,
                DoanhThuNgay = newRevenueDto.DoanhThuNgay,
                NgayCapNhat = newRevenueDto.NgayCapNhat
                
            };
            // Thêm bản ghi doanh thu mới vào cơ sở dữ liệu
            _context.DoanhThus.Add(newRevenue);
            await _context.SaveChangesAsync();

            // Trả về bản ghi doanh thu vừa tạo
            return CreatedAtAction(nameof(GetById), new { id = newRevenue.MaDoanhThu }, newRevenue);
        }

        /// <summary>
        /// Cập nhật thông tin của một bản ghi doanh thu dựa vào ID.
        /// </summary>
        /// <param name="id">ID của bản ghi doanh thu cần cập nhật.</param>
        /// <param name="updatedRevenue">Thông tin mới của bản ghi doanh thu.</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRevenue(int id, DoanhThuDto updatedRevenue)
        {
            var existingRevenue = await _context.DoanhThus.FindAsync(id);
            if (existingRevenue == null)
            {
                return NotFound("Revenue record not found.");
            }

            // Cập nhật các thuộc tính của bản ghi doanh thu
            existingRevenue.MaSanPham = updatedRevenue.MaSanPham;
            existingRevenue.TongPhiNhap = updatedRevenue.TongPhiNhap;
            existingRevenue.TongPhiXuat = updatedRevenue.TongPhiXuat;
            existingRevenue.PhiVanHanh = updatedRevenue.PhiVanHanh;
            existingRevenue.DoanhThuNgay = updatedRevenue.DoanhThuNgay;
            existingRevenue.NgayCapNhat = updatedRevenue.NgayCapNhat;

            try
            {
                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.DoanhThus.AnyAsync(dt => dt.MaDoanhThu == id))
                {
                    return NotFound("Revenue record not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa một bản ghi doanh thu dựa vào ID.
        /// </summary>
        /// <param name="id">ID của bản ghi doanh thu cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRevenue(int id)
        {
            var revenue = await _context.DoanhThus.FindAsync(id);
            if (revenue == null)
            {
                return NotFound("Revenue record not found.");
            }

            // Xóa bản ghi doanh thu khỏi cơ sở dữ liệu
            _context.DoanhThus.Remove(revenue);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Tính doanh thu ngày từ chi tiết phiếu xuất, phiếu nhập và phí vận hành, và lưu vào cơ sở dữ liệu.
        /// </summary>
        /// <returns>Trả về bản ghi doanh thu đã được tạo mới.</returns>
        [HttpPost("calculate-and-create-revenue")]
        public async Task<ActionResult<DoanhThu>> CalculateAndCreateRevenue()
        {
            // Lấy tất cả các chi tiết phiếu xuất hàng
            var exportOrderDetails = await _context.ChiTietPhieuXuatHangs
                .Include(c => c.MaSanPhamNavigation)  // Bao gồm thông tin sản phẩm
                .Include(c => c.MaPhieuXuatHangNavigation)  // Bao gồm thông tin phiếu xuất
                .Where(c => c.MaPhieuXuatHangNavigation.NgayXuat.HasValue) // Lọc theo phiếu có ngày cập nhật
                .ToListAsync();

            // Lấy tất cả các sản phẩm để lấy đơn giá
            var products = await _context.SanPhams.ToListAsync();

            // Tính doanh thu từ chi tiết phiếu xuất hàng
            var revenueFromExportDetails = exportOrderDetails.GroupBy(c => c.MaSanPham)
                .Select(g => new
                {
                    SanPham = g.Key,
                    TenSanPham = g.FirstOrDefault().MaSanPhamNavigation.TenSanPham,
                    // Tính tổng phí xuất từ chi tiết phiếu xuất (Số lượng * Đơn giá xuất)
                    TotalExportFee = g.Sum(c => (c.DonGiaXuat ?? 0) * (c.SoLuong ?? 0)),
                    NgayCapNhatXuat = g.Max(c => c.MaPhieuXuatHangNavigation.NgayXuat) // Ngày cập nhật phiếu xuất mới nhất
                })
                .ToList();

            // Tính tổng phí nhập từ chi tiết phiếu xuất (Số lượng * Đơn giá trong sản phẩm)
            var revenueFromImportDetails = exportOrderDetails.GroupBy(c => c.MaSanPham)
                .Select(g => new
                {
                    SanPham = g.Key,
                    TenSanPham = g.FirstOrDefault().MaSanPhamNavigation.TenSanPham,
                    // Tính tổng phí nhập từ số lượng trong chi tiết phiếu xuất * Đơn giá sản phẩm
                    TotalImportFee = g.Sum(c => (c.SoLuong ?? 0) * (c.MaSanPhamNavigation.DonGia ?? 0)), // Lấy Đơn giá trong sản phẩm
                })
                .ToList();

            // Tính doanh thu ngày và kết hợp các dữ liệu
            var totalRevenue = new List<DoanhThu>();

            foreach (var exportRevenue in revenueFromExportDetails)
            {
                var correspondingImportRevenue = revenueFromImportDetails
                    .FirstOrDefault(r => r.SanPham == exportRevenue.SanPham);

                var totalRevenueDay = exportRevenue.TotalExportFee - (correspondingImportRevenue?.TotalImportFee ?? 0);
                var operatingFee = 1000;  // Ví dụ phí vận hành
                var finalRevenue = totalRevenueDay - operatingFee;

                // Tạo bản ghi doanh thu mới
                var newRevenue = new DoanhThu
                {
                    MaSanPham = exportRevenue.SanPham,
                    TongPhiNhap = correspondingImportRevenue?.TotalImportFee ?? 0,
                    TongPhiXuat = exportRevenue.TotalExportFee,
                    PhiVanHanh = operatingFee,
                    DoanhThuNgay = finalRevenue,
                    NgayCapNhat = DateTime.Now // Ngày tính toán doanh thu
                };

                _context.DoanhThus.Add(newRevenue); // Thêm bản ghi doanh thu mới vào cơ sở dữ liệu
                totalRevenue.Add(newRevenue); // Thêm vào danh sách trả về
            }

            await _context.SaveChangesAsync(); // Lưu tất cả các thay đổi vào cơ sở dữ liệu

            return Ok(totalRevenue); // Trả về danh sách các bản ghi doanh thu vừa tạo
        }

    }
}
