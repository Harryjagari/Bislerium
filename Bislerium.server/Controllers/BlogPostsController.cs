using Bislerium.server.Data;
using Bislerium.server.Data.Entities;
using Bislerium.shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Bislerium.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly DataContext _context;

        public BlogPostsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts()
        {
            var blogPosts = await _context.BlogPosts.Select(bp => new BlogPost
            {
                Id = bp.Id,
                Title = bp.Title,
                Body = bp.Body,
                AuthorId = bp.AuthorId,
                ImageUrl = GetImagePath(bp.ImageUrl)
            }).ToListAsync();

            return Ok(blogPosts);
        }

        private static string GetImagePath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return null;
            }

            string imageName = Path.GetFileName(imagePath);
            return imageName;
        }

        [HttpGet("image/{imageName}")]
        public IActionResult GetBlogPostImage(string imageName)
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", imageName);

            if (System.IO.File.Exists(imagePath))
            {
                var imageFileStream = System.IO.File.OpenRead(imagePath);
                return File(imageFileStream, "image/jpeg");
            }
            else
            {
                return NotFound();
            }
        }


        [HttpGet("{id}")]
        public IActionResult GetBlogPost(Guid id)
        {
            var blogPost = _context.BlogPosts.Find(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return Ok(blogPost);
        }


        [HttpPost]
        [Authorize(Roles = "Blogger")]
        public async Task<IActionResult> PostBlogPost([FromForm] BlogPostCreateModel blogPostCreateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string uniqueFileName = null;

            if (blogPostCreateModel.Image != null)
            {
                // Limit file size to 10 MB
                if (blogPostCreateModel.Image.Length > 3 * 1024 * 1024)
                    return BadRequest("Image size exceeds the limit");

                // Generate unique file name
                uniqueFileName = $"{Guid.NewGuid()}_{blogPostCreateModel.Image.FileName}";

                string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                string filePath = Path.Combine(uploadDir, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await blogPostCreateModel.Image.CopyToAsync(stream);
                }
            }

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var blogPost = new BlogPost
            {
                Title = blogPostCreateModel.Title,
                Body = blogPostCreateModel.Body,
                ImageUrl = uniqueFileName,
                AuthorId = userId,
                CreationDate = DateTime.Now
            };

            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBlogPost), new { id = blogPost.Id }, blogPost);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Blogger")]
        public async Task<IActionResult> PutBlogPost(int id, [FromForm] BlogPostUpdateModel blogPostUpdateModel)
        {
            if (id != blogPostUpdateModel.Id)
            {
                return BadRequest();
            }

            var blogPost = _context.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (blogPost.AuthorId != userId)
            {
                return Forbid();
            }

            if (blogPostUpdateModel.Image != null)
            {
                // Limit file size to 3 MB
                const int maxFileSizeInBytes = 3 * 1024 * 1024; 

                if (blogPostUpdateModel.Image.Length > maxFileSizeInBytes)
                {
                    ModelState.AddModelError("Image", "The image file size cannot exceed 3 megabytes (MB).");
                    return BadRequest(ModelState);
                }

                string uniqueFileName = $"{Guid.NewGuid()}_{blogPostUpdateModel.Image.FileName}";

                string relativeFilePath = Path.Combine("images", uniqueFileName);

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativeFilePath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await blogPostUpdateModel.Image.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(blogPost.ImageUrl))
                {
                    string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", blogPost.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                blogPost.ImageUrl = relativeFilePath;
            }

            blogPost.Title = blogPostUpdateModel.Title;
            blogPost.Body = blogPostUpdateModel.Body;

            _context.Entry(blogPost).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Blogger")]
        public IActionResult DeleteBlogPost(Guid id)
        {
            var blogPost = _context.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (blogPost.AuthorId != userId)
            {
                return Forbid();
            }

            if (!string.IsNullOrEmpty(blogPost.ImageUrl))
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", blogPost.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.BlogPosts.Remove(blogPost);
            _context.SaveChanges();

            return NoContent();
        }

        private bool BlogPostExists(Guid id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}
