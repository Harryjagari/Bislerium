using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bislerium.server.Data;
using Bislerium.server.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Bislerium.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public AdminDashboardController(UserManager<User> userManager, DataContext context)
        {
            _context = context;
            _userManager = userManager;
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

        [HttpGet("top-posts-all-time")]
        public async Task<IActionResult> GetTopPostsAllTime()
        {
            try
            {
                var topPostsAllTime = await _context.BlogPosts
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

                return Ok(topPostsAllTime);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("top-bloggers-all-time")]
        public async Task<IActionResult> GetTopBloggersAllTime()
        {
            try
            {
                // Fetch all users
                var allUsers = await _context.Users.ToListAsync();

                // Filter out users who are not in the "Admin" role
                var topBloggersAllTime = allUsers
                    .Where(u => !_userManager.IsInRoleAsync(u, "Admin").Result)
                    .OrderByDescending(u => u.BlogPosts.Count)
                    .Take(10)
                    .Select(u => new
                    {
                        u.Id,
                        u.UserName,
                        TotalBlogPosts = u.BlogPosts.Count,
                        TotalComments = u.Comments.Count
                    })
                    .ToList();

                return Ok(topBloggersAllTime);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet("top-posts-monthly")]
        public async Task<IActionResult> GetTopPostsMonthly()
        {
            try
            {
                var now = DateTime.Now;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var topPostsMonthly = await _context.BlogPosts
                    .Where(p => p.CreationDate >= startOfMonth && p.CreationDate <= endOfMonth)
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

                return Ok(topPostsMonthly);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("top-bloggers-monthly")]
        public async Task<IActionResult> GetTopBloggersMonthly()
        {
            try
            {
                var now = DateTime.Now;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                // Fetch all users
                var allUsers = await _context.Users.Include(u => u.BlogPosts).Include(u => u.Comments).ToListAsync();

                // Filter out users who have the "Admin" role asynchronously
                var filteredUsers = new List<User>();
                foreach (var user in allUsers)
                {
                    var isInRole = await _userManager.IsInRoleAsync(user, "Admin");
                    if (!isInRole)
                    {
                        filteredUsers.Add(user);
                    }
                }

                // Filter out users who have blog posts within the current month
                var topBloggersMonthly = filteredUsers
                    .Where(u => u.BlogPosts.Any(p => p.CreationDate >= startOfMonth && p.CreationDate <= endOfMonth))
                    .OrderByDescending(u => u.BlogPosts.Count(p => p.CreationDate >= startOfMonth && p.CreationDate <= endOfMonth))
                    .Take(10)
                    .Select(u => new
                    {
                        u.Id,
                        u.UserName,
                        TotalBlogPosts = u.BlogPosts.Count,
                        TotalComments = u.Comments.Count
                    })
                    .ToList();

                return Ok(topBloggersMonthly);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}
