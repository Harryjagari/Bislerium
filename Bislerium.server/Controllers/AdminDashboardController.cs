using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bislerium.server.Data;
using Bislerium.server.Data.Entities;

namespace Bislerium.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly DataContext _context;

        public AdminDashboardController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("all-time-stats")]
        public async Task<IActionResult> GetAllTimeStats()
        {
            try
            {
                var allTimeStats = new
                {
                    TotalBlogPosts = await _context.BlogPosts.CountAsync(),
                    TotalUpvotes = await _context.Reactions.CountAsync(r => r.Type == ReactionType.Upvote),
                    TotalDownvotes = await _context.Reactions.CountAsync(r => r.Type == ReactionType.Downvote),
                    TotalComments = await _context.Comments.CountAsync()
                };

                return Ok(allTimeStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("monthly-stats")]
        public async Task<IActionResult> GetMonthlyStats()
        {
            try
            {
                var now = DateTime.Now;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var monthlyStats = new
                {
                    TotalBlogPosts = await _context.BlogPosts.CountAsync(p => p.CreationDate >= startOfMonth && p.CreationDate <= endOfMonth),
                    TotalUpvotes = await _context.Reactions.CountAsync(r => r.Type == ReactionType.Upvote && r.CreationDate >= startOfMonth && r.CreationDate <= endOfMonth),
                    TotalDownvotes = await _context.Reactions.CountAsync(r => r.Type == ReactionType.Downvote && r.CreationDate >= startOfMonth && r.CreationDate <= endOfMonth),
                    TotalComments = await _context.Comments.CountAsync(c => c.CreationDate >= startOfMonth && c.CreationDate <= endOfMonth)
                };

                return Ok(monthlyStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("top-posts")]
        public async Task<IActionResult> GetTopPosts()
        {
            try
            {
                var topPosts = await _context.BlogPosts
                    .OrderByDescending(p => p.Reactions.Count)
                    .Take(10)
                    .Select(p => new
                    {
                        p.Id,
                        p.Title,
                        Upvotes = p.Reactions.Count(r => r.Type == ReactionType.Upvote),
                        Downvotes = p.Reactions.Count(r => r.Type == ReactionType.Downvote),
                        TotalComments = p.Comments.Count
                    })
                    .ToListAsync();

                return Ok(topPosts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("top-bloggers")]
        public async Task<IActionResult> GetTopBloggers()
        {
            try
            {
                var topBloggers = await _context.Users
                    .OrderByDescending(u => u.BlogPosts.Count)
                    .Take(10)
                    .Select(u => new
                    {
                        u.Id,
                        u.FullName,
                        TotalBlogPosts = u.BlogPosts.Count,
                        TotalComments = u.Comments.Count
                    })
                    .ToListAsync();

                return Ok(topBloggers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
