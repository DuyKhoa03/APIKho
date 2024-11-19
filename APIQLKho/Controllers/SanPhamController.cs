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
        public async Task<ActionResult<IEnumerable<SanPhamDto>>> Get()
        {
            var products = await _context.SanPhams
                                         .Include(sp => sp.MaLoaiSanPhamNavigation)
                                         .Include(sp => sp.MaHangSanXuatNavigation)
                                         .Select(sp => new SanPhamDto
                                         {
                                             MaSanPham = sp.MaSanPham,
                                             TenSanPham = sp.TenSanPham,
                                             Mota = sp.Mota,
                                             SoLuong = sp.SoLuong,
                                             DonGia = sp.DonGia,
                                             XuatXu = sp.XuatXu,
                                             Image = sp.Image,
                                             MaLoaiSanPham = sp.MaLoaiSanPham,
                                             TenLoaiSanPham = sp.MaLoaiSanPhamNavigation.TenLoaiSanPham,
                                             MaHangSanXuat = sp.MaHangSanXuat,
                                             TenHangSanXuat = sp.MaHangSanXuatNavigation.TenHangSanXuat
                                         })
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
        public async Task<ActionResult<SanPhamDto>> GetById(int id)
        {
            var product = await _context.SanPhams
                                        .Include(sp => sp.MaLoaiSanPhamNavigation)
                                        .Include(sp => sp.MaHangSanXuatNavigation)
                                        .Where(sp => sp.MaSanPham == id)
                                        .Select(sp => new SanPhamDto
                                        {
                                            MaSanPham = sp.MaSanPham,
                                            TenSanPham = sp.TenSanPham,
                                            Mota = sp.Mota,
                                            SoLuong = sp.SoLuong,
                                            DonGia = sp.DonGia,
                                            XuatXu = sp.XuatXu,
                                            Image = sp.Image,
                                            MaLoaiSanPham = sp.MaLoaiSanPham,
                                            TenLoaiSanPham = sp.MaLoaiSanPhamNavigation.TenLoaiSanPham,
                                            MaHangSanXuat = sp.MaHangSanXuat,
                                            TenHangSanXuat = sp.MaHangSanXuatNavigation.TenHangSanXuat
                                        })
                                        .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(product);
        }
        [HttpPost]
        [Route("uploadfile")]
        public async Task<ActionResult<SanPham>> CreateProductWithImage([FromForm] SanPhamDto newProductDto)
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
                MaLoaiSanPham = newProductDto.MaLoaiSanPham,
                MaHangSanXuat = newProductDto.MaHangSanXuat
            };

            // Xử lý ảnh tải lên
            if (newProductDto.Img != null && newProductDto.Img.Length > 0)
            {
                var fileName = Path.GetFileName(newProductDto.Img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await newProductDto.Img.CopyToAsync(stream);
                }

                // Giả sử bạn có trường `Image` trong `SanPham` để lưu đường dẫn ảnh
                newProduct.Image = "/UploadedImages/" + fileName;
            }
            else
            {
                newProduct.Image = ""; // Trường hợp không có ảnh
            }

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
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] SanPhamDto updatedProductDto)
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

            // Cập nhật các thuộc tính của sản phẩm (không bao gồm ảnh)
            existingProduct.TenSanPham = updatedProductDto.TenSanPham;
            existingProduct.Mota = updatedProductDto.Mota;
            existingProduct.SoLuong = updatedProductDto.SoLuong;
            existingProduct.DonGia = updatedProductDto.DonGia;
            existingProduct.XuatXu = updatedProductDto.XuatXu;
            existingProduct.MaLoaiSanPham = updatedProductDto.MaLoaiSanPham;
            existingProduct.MaHangSanXuat = updatedProductDto.MaHangSanXuat;

            // Xử lý ảnh nếu có tải lên ảnh mới
            if (updatedProductDto.Img != null && updatedProductDto.Img.Length > 0)
            {
                // Đường dẫn ảnh cũ
                var oldImagePath = existingProduct.Image;

                // Lưu ảnh mới
                var fileName = Path.GetFileName(updatedProductDto.Img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updatedProductDto.Img.CopyToAsync(stream);
                }

                // Cập nhật đường dẫn ảnh mới
                existingProduct.Image = "/UploadedImages/" + fileName;

                // Xóa ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(oldImagePath))
                {
                    var fullOldImagePath = Path.Combine(Directory.GetCurrentDirectory(), oldImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullOldImagePath))
                    {
                        System.IO.File.Delete(fullOldImagePath);
                    }
                }
            }

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
        public async Task<ActionResult<IEnumerable<SanPhamDto>>> Search(string keyword)
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
