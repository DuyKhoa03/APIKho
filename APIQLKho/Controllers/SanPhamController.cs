using APIQLKho.Dtos;
using APIQLKho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using ZXing;

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
                                         .Where(sp => sp.Hide == false || sp.Hide == null)  // Chỉ lấy sản phẩm không bị ẩn
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
                                             MaVach = sp.MaVach,
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
                                        .Where(sp => sp.MaSanPham == id && (sp.Hide == false || sp.Hide == null))  // Chỉ lấy sản phẩm không bị ẩn
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
                                            MaVach = sp.MaVach,
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
        public async Task<ActionResult<SanPham>> CreateProduct([FromForm] SanPhamDto newProductDto)
        {
            if (newProductDto == null)
            {
                return BadRequest("Product data is null.");
            }

            // Tạo sản phẩm mới
            var newProduct = new SanPham
            {
                TenSanPham = newProductDto.TenSanPham,
                Mota = newProductDto.Mota,
                SoLuong = newProductDto.SoLuong,
                DonGia = newProductDto.DonGia,
                XuatXu = newProductDto.XuatXu,
                MaLoaiSanPham = newProductDto.MaLoaiSanPham,
                MaHangSanXuat = newProductDto.MaHangSanXuat,
                Hide = false
            };

            // Xử lý ảnh tải lên (nếu có)
            if (newProductDto.Img != null && newProductDto.Img.Length > 0)
            {
                var fileName = Path.GetFileName(newProductDto.Img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await newProductDto.Img.CopyToAsync(stream);
                }

                newProduct.Image = "/UploadedImages/" + fileName;
            }
            else
            {
                newProduct.Image = ""; // Trường hợp không có ảnh
            }

            // Thêm sản phẩm mới vào cơ sở dữ liệu trước để có thể sử dụng MaSanPham
            _context.SanPhams.Add(newProduct);
            await _context.SaveChangesAsync();

            try
            {
                // Tạo mã vạch
                var barcodeWriter = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = 150,
                        Width = 300,
                        Margin = 10
                    }
                };

                var pixelData = barcodeWriter.Write(newProduct.MaSanPham.ToString());

                // Lưu mã vạch dưới dạng hình ảnh
                var barcodeFileName = $"barcode_{newProduct.MaSanPham}.png";
                var barcodeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", barcodeFileName);

                using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
                {
                    var bitmapData = bitmap.LockBits(
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.WriteOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    // Lưu file hình ảnh mã vạch
                    bitmap.Save(barcodeFilePath, ImageFormat.Png);
                }

                // Cập nhật đường dẫn mã vạch vào sản phẩm
                newProduct.MaVach = "/UploadedImages/" + barcodeFileName;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu tạo mã vạch thất bại
                _logger.LogError(ex, "Error generating barcode");
                throw;
            }

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

            // Cập nhật trường Hide thành true thay vì xóa sản phẩm
            product.Hide = true;

            try
            {
                await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
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
                .Where(sp => sp.Hide == false || sp.Hide == null)
                                               .Include(sp => sp.MaLoaiSanPhamNavigation)
                                               .Include(sp => sp.MaHangSanXuatNavigation)
                                               .Where(sp => sp.TenSanPham.Contains(keyword) || sp.Mota.Contains(keyword))
                                               .ToListAsync();

            return Ok(searchResults);
        }
        [HttpGet("search-by-barcode/{barcode}")]
        public async Task<ActionResult<SanPhamDto>> GetByBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
            {
                return BadRequest("Barcode is empty.");
            }

            // Tìm sản phẩm dựa trên mã vạch
            var product = await _context.SanPhams
                                        .Where(sp => sp.MaVach == barcode && (sp.Hide == false || sp.Hide == null))
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
                                        .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(product);
        }

    }
}
