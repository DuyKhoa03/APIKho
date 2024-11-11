using APIQLKho.Dtos;
using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SanPhamController : ControllerBase
    {
        private readonly ILogger<SanPhamController> _logger;
        private readonly QlkhohangContext _context;

        public SanPhamController(ILogger<SanPhamController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả các sản phẩm.
        /// </summary>
        /// <returns>Một danh sách các sản phẩm, bao gồm thông tin loại sản phẩm và hãng sản xuất.</returns>
        // GET: api/sanpham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPham>>> Get()
        {
            var products = await _context.SanPhams
                                         .Include(sp => sp.MaLoaiSanPhamNavigation) // Bao gồm thông tin loại sản phẩm
                                         .Include(sp => sp.MaHangSanXuatNavigation) // Bao gồm thông tin hãng sản xuất
                                         .ToListAsync();
            return Ok(products);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một sản phẩm dựa vào ID.
        /// </summary>
        /// <param name="id">ID của sản phẩm cần lấy thông tin.</param>
        /// <returns>Thông tin chi tiết của sản phẩm nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        // GET: api/sanpham/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SanPham>> GetById(int id)
        {
            var product = await _context.SanPhams
                                        .Include(sp => sp.MaLoaiSanPhamNavigation)
                                        .Include(sp => sp.MaHangSanXuatNavigation)
                                        .FirstOrDefaultAsync(sp => sp.MaSanPham == id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(product);
        }

        /// <summary>
        /// Thêm mới một sản phẩm vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="newProductDto">Thông tin sản phẩm mới cần thêm (dữ liệu từ DTO).</param>
        /// <returns>Sản phẩm vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
        // POST: api/sanpham
        [HttpPost]
        public async Task<ActionResult<SanPham>> CreateProduct(SanPhamDto newProductDto)
        {
            if (newProductDto == null)
            {
                return BadRequest("Product data is null.");
            }

            var newProduct = new SanPham
            {
                TenSanPham = newProductDto.TenSanPham,
                Mota = newProductDto.Mota,
                SoLuong = newProductDto.SoLuong,
                DonGia = newProductDto.DonGia,
                XuatXu = newProductDto.XuatXu,
                Image = newProductDto.Image,
                MaLoaiSanPham = newProductDto.MaLoaiSanPham,
                MaHangSanXuat = newProductDto.MaHangSanXuat
            };

            _context.SanPhams.Add(newProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newProduct.MaSanPham }, newProduct);
        }

        /// <summary>
        /// Cập nhật thông tin của một sản phẩm dựa vào ID.
        /// </summary>
        /// <param name="id">ID của sản phẩm cần cập nhật.</param>
        /// <param name="updatedProductDto">Thông tin sản phẩm cần cập nhật (dữ liệu từ DTO).</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        // PUT: api/sanpham/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, SanPhamDto updatedProductDto)
        {
            if (updatedProductDto == null)
            {
                return BadRequest("Product data is null.");
            }

            var existingProduct = await _context.SanPhams.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            // Cập nhật các thuộc tính của sản phẩm
            existingProduct.TenSanPham = updatedProductDto.TenSanPham;
            existingProduct.Mota = updatedProductDto.Mota;
            existingProduct.SoLuong = updatedProductDto.SoLuong;
            existingProduct.DonGia = updatedProductDto.DonGia;
            existingProduct.XuatXu = updatedProductDto.XuatXu;
            existingProduct.Image = updatedProductDto.Image;
            existingProduct.MaLoaiSanPham = updatedProductDto.MaLoaiSanPham;
            existingProduct.MaHangSanXuat = updatedProductDto.MaHangSanXuat;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.SanPhams.AnyAsync(sp => sp.MaSanPham == id))
                {
                    return NotFound("Product not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa một sản phẩm dựa vào ID.
        /// </summary>
        /// <param name="id">ID của sản phẩm cần xóa.</param>
        /// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
        // DELETE: api/sanpham/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.SanPhams.FindAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            // Xóa sản phẩm khỏi cơ sở dữ liệu
            _context.SanPhams.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Tìm kiếm sản phẩm theo tên hoặc mô tả.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm.</param>
        /// <returns>Danh sách các sản phẩm phù hợp với từ khóa.</returns>
        // GET: api/sanpham/search
        [HttpGet("{keyword}")]
        public async Task<ActionResult<IEnumerable<SanPham>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var searchResults = await _context.SanPhams
                                               .Include(sp => sp.MaLoaiSanPhamNavigation)
                                               .Include(sp => sp.MaHangSanXuatNavigation)
                                               .Where(sp => sp.TenSanPham.Contains(keyword) || sp.Mota.Contains(keyword))
                                               .ToListAsync();

            return Ok(searchResults);
        }

    }
}
