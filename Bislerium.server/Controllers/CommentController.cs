using Bislerium.server.Data;
using Bislerium.server.Data.Entities;
using Bislerium.server.SignalR.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bislerium.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IHubContext<CommentHub, ICommentHubClient> _commentHubContext;
        private readonly UserManager<User> _userManager;

        public CommentController(UserManager<User> userManager, DataContext context, IHubContext<CommentHub, ICommentHubClient> commentHubContext)
        {
            _context = context;
            _commentHubContext = commentHubContext;
            _userManager = userManager;
        }


        [HttpGet("{postId}/comments")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsForPost(Guid postId)
        {
            var post = await _context.BlogPosts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                return NotFound();
            }

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var json = JsonSerializer.Serialize(post.Comments.ToList(), options);

            return Content(json, "application/json");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(Guid postId, string comment)
        {
            var post = await _context.BlogPosts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (nameIdentifierClaim == null)
            {
                return Unauthorized();
            }

            string userId = nameIdentifierClaim.Value;

            var newComment = new Comment
            {
                Content = comment,
                AuthorId = userId,
                BlogPostId = postId,
                CreationDate = DateTime.Now
            };
            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();

            // Notify the post author
            var postAuthor = await _userManager.FindByIdAsync(post.AuthorId);
            if (postAuthor != null)
            {
                var notificationMessage = $"{postAuthor.UserName} has commented on your post '{post.Title}' at {DateTime.Now}.";
                await _commentHubContext.Clients.User(post.AuthorId).ReceiveCommentNotification(postId, notificationMessage);
            }

            return Ok();
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(Guid id, string updatedComment)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (comment.AuthorId != userId)
            {
                return Forbid();
            }

            // Create a new entry in the update history
            var updateHistoryEntry = new CommentUpdateHistory
            {
                CommentId = comment.Id,
                OriginalContent = comment.Content,
                UpdatedContent = updatedComment,
                Timestamp = DateTime.Now
            };

            _context.CommentUpdateHistories.Add(updateHistoryEntry);

            comment.Content = updatedComment;
            await _context.SaveChangesAsync();

            var post = await _context.BlogPosts.FindAsync(comment.BlogPostId);
            var postAuthor = await _userManager.FindByIdAsync(userId);

            if (postAuthor != null)
            {
                var notificationMessage = $"{postAuthor.UserName} has updated their comment on your post at {DateTime.Now}.";
                await _commentHubContext.Clients.User(comment.BlogPost.AuthorId).ReceiveCommentNotification(comment.BlogPostId, notificationMessage);
            }

            return Ok();
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (comment.AuthorId != userId)
            {
                return Forbid();
            }

            var postId = comment.BlogPostId;
            var post = await _context.BlogPosts.FindAsync(postId);

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            var postAuthor = await _userManager.FindByIdAsync(userId);

            if (postAuthor != null)
            {
                var notificationMessage = $"{postAuthor.UserName} has deleted their comment on your post at {DateTime.Now}.";
                await _commentHubContext.Clients.User(comment.BlogPost.AuthorId).ReceiveCommentNotification(postId, notificationMessage);
            }

            return Ok();
        }
    }
}
