using APIQLKho.Dtos;
using APIQLKho.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Controllers
{
    //TEST
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BlogController : ControllerBase
    {
        private readonly ILogger<BlogController> _logger;
        private readonly QlkhohangContext _context;
        private readonly Cloudinary _cloudinary;

        public BlogController(ILogger<BlogController> logger, QlkhohangContext context, Cloudinary cloudinary)
        {
            _logger = logger;
            _context = context;
            _cloudinary = cloudinary;
        }


        /// <summary>
        /// Lấy danh sách tất cả các blog, bao gồm thông tin người dùng nếu cần, và chỉ lấy các blog không bị ẩn.
        /// </summary>
        /// <returns>Danh sách các blog không bị ẩn.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogDto>>> Get()
        {
            var blogs = await _context.Blogs
                                      .Include(b => b.MaNguoiDungNavigation)
                                      .Where(b => (bool)!b.Hide)
                                      .Select(b => new BlogDto
                                      {
                                          BlogId = b.BlogId,
                                          Anh = b.Anh,
                                          Mota = b.Mota,
                                          Link = b.Link,
                                          Hide = b.Hide,
                                          MaNguoiDung = b.MaNguoiDung,
                                          TenNguoiDung = b.MaNguoiDungNavigation.TenNguoiDung // Thêm tên người dùng
                                      })
                                      .ToListAsync();

            return Ok(blogs);
        }


        /// <summary>
        /// Lấy thông tin chi tiết của một blog dựa vào ID.
        /// </summary>
        /// <param name="id">ID của blog cần lấy.</param>
        /// <returns>Thông tin của blog nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDto>> GetById(int id)
        {
            var blog = await _context.Blogs
                                     .Include(b => b.MaNguoiDungNavigation)
                                     .Where(b => b.BlogId == id && b.Hide == false)
                                     .Select(b => new BlogDto
                                     {
                                         BlogId = b.BlogId,
                                         Anh = b.Anh,
                                         Mota = b.Mota,
                                         Link = b.Link,
                                         Hide = b.Hide,
                                         MaNguoiDung = b.MaNguoiDung,
                                         TenNguoiDung = b.MaNguoiDungNavigation.TenNguoiDung
                                     })
                                     .FirstOrDefaultAsync();

            if (blog == null)
            {
                return NotFound("Blog not found.");
            }

            return Ok(blog);
        }


        /// <summary>
        /// Tạo mới một blog.
        /// </summary>
        /// <param name="newBlogDto">Thông tin của blog mới cần tạo.</param>
        /// <returns>Blog vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
        
        [HttpPost]
        [Route("uploadfile")]
        public async Task<IActionResult> CreateWithImage([FromForm] BlogDto blogDto)
        {
            if (blogDto == null)
            {
                return BadRequest("Blog data is null.");
            }

            var newBlog = new Blog
            {
                Mota = blogDto.Mota,
                Link = blogDto.Link,
                Hide = blogDto.Hide,
                MaNguoiDung = blogDto.MaNguoiDung
            };

            // Upload ảnh lên Cloudinary
            if (blogDto.Image != null && blogDto.Image.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(blogDto.Image.FileName, blogDto.Image.OpenReadStream()),
                    Folder = "blog-images", // Tên thư mục Cloudinary
                    Transformation = new Transformation().Crop("limit").Width(800).Height(800)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BadRequest("Failed to upload image to Cloudinary.");
                }

                // Gán URL ảnh từ Cloudinary
                newBlog.Anh = uploadResult.SecureUrl.ToString();
            }

            _context.Blogs.Add(newBlog);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newBlog.BlogId }, newBlog);
        }


        /// <summary>
        /// Cập nhật thông tin của một blog dựa vào ID.
        /// </summary>
        /// <param name="id">ID của blog cần cập nhật.</param>
        /// <param name="updatedBlogDto">Thông tin mới của blog.</param>
        /// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] BlogDto updatedBlogDto)
        {
            if (updatedBlogDto == null)
            {
                return BadRequest("Blog data is null.");
            }

            var existingBlog = await _context.Blogs.FindAsync(id);
            if (existingBlog == null)
            {
                return NotFound("Blog not found.");
            }

            // Cập nhật các thuộc tính không liên quan đến ảnh
            existingBlog.Mota = updatedBlogDto.Mota;
            existingBlog.Link = updatedBlogDto.Link;
            existingBlog.Hide = updatedBlogDto.Hide;
            existingBlog.MaNguoiDung = updatedBlogDto.MaNguoiDung;

            // Upload ảnh mới lên Cloudinary
            if (updatedBlogDto.Image != null && updatedBlogDto.Image.Length > 0)
            {
                // Xóa ảnh cũ trên Cloudinary nếu có
                if (!string.IsNullOrEmpty(existingBlog.Anh))
                {
                    var publicId = new Uri(existingBlog.Anh).Segments.Last().Split('.')[0]; // Trích xuất Public ID từ URL
                    var deletionParams = new DeletionParams(publicId);
                    await _cloudinary.DestroyAsync(deletionParams);
                }

                // Tải ảnh mới lên Cloudinary
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(updatedBlogDto.Image.FileName, updatedBlogDto.Image.OpenReadStream()),
                    Folder = "blog-images",
                    Transformation = new Transformation().Crop("limit").Width(800).Height(800)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BadRequest("Failed to upload image to Cloudinary.");
                }

                existingBlog.Anh = uploadResult.SecureUrl.ToString();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Blogs.AnyAsync(b => b.BlogId == id))
                {
                    return NotFound("Blog not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Ẩn một blog thay vì xóa hoàn toàn khỏi cơ sở dữ liệu.
        /// </summary>
        /// <param name="id">ID của blog cần ẩn.</param>
        /// <returns>Không trả về nội dung nếu ẩn thành công; nếu không, trả về thông báo lỗi.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound("Blog not found.");
            }

            // Xóa ảnh trên Cloudinary nếu có
            if (!string.IsNullOrEmpty(blog.Anh))
            {
                var publicId = new Uri(blog.Anh).Segments.Last().Split('.')[0];
                var deletionParams = new DeletionParams(publicId);
                await _cloudinary.DestroyAsync(deletionParams);
            }

            // Xóa blog khỏi database
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Tìm kiếm các blog dựa trên từ khóa trong mô tả hoặc tên người dùng.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (trong mô tả hoặc tên người dùng).</param>
        /// <returns>Danh sách các blog có chứa từ khóa và không bị ẩn.</returns>
        // GET: api/blog/search/{keyword}
        [HttpGet("{keyword}")]
        public async Task<ActionResult<IEnumerable<BlogDto>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var searchResults = await _context.Blogs
                                              .Include(b => b.MaNguoiDungNavigation)
                                              .Where(b => (b.Mota.Contains(keyword) || b.MaNguoiDungNavigation.TenNguoiDung.Contains(keyword)) && b.Hide == false)
                                              .Select(b => new BlogDto
                                              {
                                                  BlogId = b.BlogId,
                                                  Anh = b.Anh,
                                                  Mota = b.Mota,
                                                  Link = b.Link,
                                                  Hide = b.Hide,
                                                  MaNguoiDung = b.MaNguoiDung,
                                                  TenNguoiDung = b.MaNguoiDungNavigation.TenNguoiDung
                                              })
                                              .ToListAsync();

            return Ok(searchResults);
        }

    }
}
