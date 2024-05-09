using Bislerium.server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bislerium.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateHistoryController : ControllerBase
    {
        private readonly DataContext _context;

        public UpdateHistoryController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("blogpost")]
        public async Task<IActionResult> GetBlogPostUpdateHistory()
        {
            var updateHistory = await _context.BlogPostUpdateHistories.ToListAsync();
            return Ok(updateHistory);
        }

        [HttpGet("comment")]
        public async Task<IActionResult> GetCommentUpdateHistory()
        {
            var updateHistory = await _context.CommentUpdateHistories.ToListAsync();
            return Ok(updateHistory);
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
    }
}
