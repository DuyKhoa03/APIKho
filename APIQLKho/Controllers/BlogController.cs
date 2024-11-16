using APIQLKho.Dtos;
using APIQLKho.Models;
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

        public BlogController(ILogger<BlogController> logger, QlkhohangContext context)
        {
            _logger = logger;
            _context = context;
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
        //[HttpPost]
        //public async Task<ActionResult<Blog>> Create(BlogDto newBlogDto)
        //{
        //    if (newBlogDto == null)
        //    {
        //        return BadRequest("Blog data is null.");
        //    }

        //    var newBlog = new Blog
        //    {
        //        Anh = newBlogDto.Anh,
        //        Mota = newBlogDto.Mota,
        //        Link = newBlogDto.Link,
        //        Hide = newBlogDto.Hide,
        //        MaNguoiDung = newBlogDto.MaNguoiDung
        //    };

        //    _context.Blogs.Add(newBlog);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetById), new { id = newBlog.BlogId }, newBlog);
        //}
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

            // Xử lý ảnh tải lên
            if (blogDto.Image != null && blogDto.Image.Length > 0)
            {
                var fileName = Path.GetFileName(blogDto.Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await blogDto.Image.CopyToAsync(stream);
                }

                newBlog.Anh = "/UploadedImages/" + fileName;
            }
            else
            {
                newBlog.Anh = ""; // Trường hợp không có ảnh
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

            // Xử lý ảnh tải lên
            if (updatedBlogDto.Image != null && updatedBlogDto.Image.Length > 0)
            {
                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(existingBlog.Anh))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), existingBlog.Anh.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Lưu ảnh mới
                var fileName = Path.GetFileName(updatedBlogDto.Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updatedBlogDto.Image.CopyToAsync(stream);
                }

                existingBlog.Anh = "/UploadedImages/" + fileName;
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

            // Xóa ảnh cũ nếu có
            if (!string.IsNullOrEmpty(blog.Anh))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), blog.Anh.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
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
