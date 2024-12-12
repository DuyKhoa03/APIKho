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
    public class NhaCungCapController : ControllerBase
    {
        private readonly ILogger<NhaCungCapController> _logger;
        private readonly QlkhohangContext _context;
        private readonly Cloudinary _cloudinary;

        public NhaCungCapController(ILogger<NhaCungCapController> logger, QlkhohangContext context, Cloudinary cloudinary)
        {
            _logger = logger;
            _context = context;
            _cloudinary = cloudinary;
        }
        /// <summary>
        /// Lấy danh sách tất cả các nhà cung cấp.
        /// </summary>
        /// <returns>Danh sách các nhà cung cấp, bao gồm thông tin về các phiếu nhập hàng liên quan.</returns>
        // GET: api/nhacungcap
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhaCungCapDto>>> Get()
        {
            var suppliers = await _context.NhaCungCaps
                                          .Where(ncc => ncc.Hide == false || ncc.Hide == null)  // Chỉ lấy nhà cung cấp không bị ẩn
                                          .Include(ncc => ncc.PhieuNhapHangs)  // Bao gồm thông tin phiếu nhập hàng
                                          .Select(ncc => new NhaCungCapDto
                                          {
                                              MaNhaCungCap = ncc.MaNhaCungCap,
                                              TenNhaCungCap = ncc.TenNhaCungCap,
                                              DiaChi = ncc.DiaChi,
                                              Email = ncc.Email,
                                              Sdt = ncc.Sdt,
                                              Image = ncc.Image
                                          })
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
        public async Task<ActionResult<NhaCungCapDto>> GetById(int id)
        {
            var supplier = await _context.NhaCungCaps
                                         .Where(ncc => ncc.MaNhaCungCap == id && (ncc.Hide == false || ncc.Hide == null))  // Chỉ lấy nhà cung cấp không bị ẩn
                                         .Include(ncc => ncc.PhieuNhapHangs)
                                         .Select(ncc => new NhaCungCapDto
                                         {
                                             MaNhaCungCap = ncc.MaNhaCungCap,
                                             TenNhaCungCap = ncc.TenNhaCungCap,
                                             DiaChi = ncc.DiaChi,
                                             Email = ncc.Email,
                                             Sdt = ncc.Sdt,
                                             Image = ncc.Image
                                         })
                                         .FirstOrDefaultAsync();

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
        [HttpPost]
        [Route("uploadfile")]
        public async Task<ActionResult<NhaCungCap>> CreateSupplierWithImage([FromForm] NhaCungCapDto newSupplierDto)
        {
            if (newSupplierDto == null)
            {
                return BadRequest("Supplier data is null.");
            }

            var newSupplier = new NhaCungCap
            {
                TenNhaCungCap = newSupplierDto.TenNhaCungCap,
                DiaChi = newSupplierDto.DiaChi,
                Email = newSupplierDto.Email,
                Sdt = newSupplierDto.Sdt,
                Hide = false
            };

            // Upload ảnh lên Cloudinary
            if (newSupplierDto.Img != null && newSupplierDto.Img.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(newSupplierDto.Img.FileName, newSupplierDto.Img.OpenReadStream()),
                    Folder = "supplier-images", // Tên thư mục Cloudinary
                    Transformation = new Transformation().Crop("limit").Width(300).Height(300)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BadRequest("Failed to upload image to Cloudinary.");
                }

                // Gán URL ảnh từ Cloudinary
                newSupplier.Image = uploadResult.SecureUrl.ToString();
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
        public async Task<IActionResult> UpdateSupplier(int id, [FromForm] NhaCungCapDto updatedSupplierDto)
        {
            var existingSupplier = await _context.NhaCungCaps.FindAsync(id);
            if (existingSupplier == null)
            {
                return NotFound("Supplier not found.");
            }

            // Cập nhật các thuộc tính không liên quan đến ảnh
            existingSupplier.TenNhaCungCap = updatedSupplierDto.TenNhaCungCap;
            existingSupplier.DiaChi = updatedSupplierDto.DiaChi;
            existingSupplier.Email = updatedSupplierDto.Email;
            existingSupplier.Sdt = updatedSupplierDto.Sdt;

            // Upload ảnh mới lên Cloudinary
            if (updatedSupplierDto.Img != null && updatedSupplierDto.Img.Length > 0)
            {
                // Xóa ảnh cũ trên Cloudinary nếu có
                if (!string.IsNullOrEmpty(existingSupplier.Image))
                {
                    var publicId = new Uri(existingSupplier.Image).Segments.Last().Split('.')[0]; // Trích xuất Public ID từ URL
                    var deletionParams = new DeletionParams(publicId);
                    await _cloudinary.DestroyAsync(deletionParams);
                }

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(updatedSupplierDto.Img.FileName, updatedSupplierDto.Img.OpenReadStream()),
                    Folder = "supplier-images",
                    Transformation = new Transformation().Crop("limit").Width(300).Height(300)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BadRequest("Failed to upload image to Cloudinary.");
                }

                existingSupplier.Image = uploadResult.SecureUrl.ToString();
            }

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

            // Xóa ảnh trên Cloudinary nếu có
            if (!string.IsNullOrEmpty(supplier.Image))
            {
                var publicId = new Uri(supplier.Image).Segments.Last().Split('.')[0]; // Trích xuất Public ID từ URL
                var deletionParams = new DeletionParams(publicId);
                await _cloudinary.DestroyAsync(deletionParams);
            }
            // Cập nhật trường Hide thành true thay vì xóa
            supplier.Hide = true;
            try
            {
                await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
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
                .Where(ncc => ncc.Hide == false || ncc.Hide == null)
                                              .Where(ncc => ncc.TenNhaCungCap.Contains(keyword) || ncc.DiaChi.Contains(keyword))
                                              .ToListAsync();

            return Ok(searchResults);
        }

    }
}
