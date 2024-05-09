using Bislerium.server.Data;
using Bislerium.server.Data.Entities;
using Bislerium.server.Utilities;
using Bislerium.shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;

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

        [HttpGet("Blogs")]
        public IActionResult GetAllBlogs()
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            IQueryable<BlogPost> blogsQuery = _context.BlogPosts
                                                    .Include(b => b.Author)
                                                    .Include(b => b.Comments)
                                                        .ThenInclude(c => c.Author)
                                                    .Include(b => b.Reactions)
                                                        .ThenInclude(r => r.User);

            var allBlogs = blogsQuery.ToList();

            return Ok(JsonSerializer.Serialize(allBlogs, options));
        }


        [HttpGet("catalogue")]
        public IActionResult GetCatalogue(string sortBy = "recency")
        {
            IQueryable<BlogPost> blogsQuery = _context.BlogPosts
                                                    .Include(b => b.Author); 

            switch (sortBy.ToLower())
            {
                case "popularity":
                    var popularBlogsQuery = _context.BlogPosts
                                              .Include(b => b.Comments)
                                              .Include(b => b.Reactions)
                                              .Include(b => b.Author); 

                    var popularBlogs = popularBlogsQuery.AsEnumerable()
                                                        .OrderByDescending(b => BlogUtility.CalculatePopularityScore(b))
                                                        .Select(b => new
                                                        {
                                                            b.Id,
                                                            b.Title,
                                                            b.Body,
                                                            b.ImageUrl,
                                                            b.AuthorId,
                                                            b.CreationDate,
                                                            AuthorName = b.Author.UserName, 
                                                            CommentsCount = b.Comments.Count,
                                                            ReactionsCount = b.Reactions.Count
                                                        })
                                                        .ToList();
                    return Ok(popularBlogs);

                case "recency":
                    blogsQuery = blogsQuery.OrderByDescending(b => b.CreationDate);
                    break;
                case "random":
                    blogsQuery = Shuffle(blogsQuery);
                    break;
                default:
                    break;
            }

            var sortedBlogs = blogsQuery.Select(b => new
            {
                b.Id,
                b.Title,
                b.Body,
                b.ImageUrl,
                b.AuthorId,
                b.CreationDate,
                AuthorName = b.Author.UserName, 
                CommentsCount = b.Comments.Count,
                ReactionsCount = b.Reactions.Count
            }).ToList();

            return Ok(sortedBlogs);
        }



        private IQueryable<T> Shuffle<T>(IQueryable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
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
        [Authorize]
        public async Task<IActionResult> PutBlogPost(Guid id, [FromForm] BlogPostUpdateModel blogPostUpdateModel)
        {
            if (id != blogPostUpdateModel.Id)
            {
                return BadRequest();
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (blogPost.AuthorId != userId)
            {
                return Forbid();
            }

            // Create a new entry in the update history
            var updateHistoryEntry = new BlogPostUpdateHistory
            {
                BlogPostId = blogPost.Id,
                OriginalTitle = blogPost.Title,
                UpdatedTitle = blogPostUpdateModel.Title,
                OriginalBody = blogPost.Body,
                UpdatedBody = blogPostUpdateModel.Body,
                OriginalImageUrl = blogPost.ImageUrl, // Store the current image URL as OriginalImageUrl
                UpdatedImageUrl = null,
                Timestamp = DateTime.Now
            };

            _context.BlogPostUpdateHistories.Add(updateHistoryEntry);

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

                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

                string newFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "blogPost-History");

                // Check if the directory exists, if not create it
                if (!Directory.Exists(newFolder))
                {
                    Directory.CreateDirectory(newFolder);
                }

                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                string newFilePath = Path.Combine(newFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await blogPostUpdateModel.Image.CopyToAsync(stream);
                }

                // Move the image file to the new folder
                System.IO.File.Move(filePath, newFilePath);

                // Update the UpdatedImageUrl with the new image file name
                updateHistoryEntry.UpdatedImageUrl = uniqueFileName;

                // Update the blog post's image URL
                blogPost.ImageUrl = uniqueFileName;
            }

            // Update the blog post's title and body
            blogPost.Title = blogPostUpdateModel.Title;
            blogPost.Body = blogPostUpdateModel.Body;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return NoContent();
        }





        [HttpDelete("{id}")]
        [Authorize]
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
