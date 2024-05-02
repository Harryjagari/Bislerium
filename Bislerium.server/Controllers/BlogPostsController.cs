using Bislerium.server.Data;
using Bislerium.server.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // GET: api/BlogPosts
        [HttpGet]
        public IActionResult GetBlogPosts()
        {
            var blogPosts = _context.BlogPosts.ToList();
            return Ok(blogPosts);
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        public IActionResult GetBlogPost(int id)
        {
            var blogPost = _context.BlogPosts.Find(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return Ok(blogPost);
        }

        // POST: api/BlogPosts
        [HttpPost]
        [Authorize(Roles = "Admin, Blogger")]
        public IActionResult PostBlogPost(BlogPost blogPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            blogPost.CreationDate = DateTime.Now;
            _context.BlogPosts.Add(blogPost);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetBlogPost), new { id = blogPost.Id }, blogPost);
        }

        // PUT: api/BlogPosts/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Blogger")]
        public IActionResult PutBlogPost(int id, BlogPost blogPost)
        {
            if (id != blogPost.Id)
            {
                return BadRequest();
            }

            _context.Entry(blogPost).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/BlogPosts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Blogger")]
        public IActionResult DeleteBlogPost(int id)
        {
            var blogPost = _context.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            _context.BlogPosts.Remove(blogPost);
            _context.SaveChanges();

            return NoContent();
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}





