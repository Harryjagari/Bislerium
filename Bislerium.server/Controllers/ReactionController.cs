using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bislerium.server.Data;
using Bislerium.server.Data.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Bislerium.server.SignalR.Hubs;
using Bislerium.server.SignalR.Interfaces;

namespace Bislerium.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "Blogger")]
    public class ReactionController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IHubContext<ReactionHub, IReactionHubClient> _reactionHubContext;

        public ReactionController(DataContext context, IHubContext<ReactionHub, IReactionHubClient> reactionHubContext)
        {
            _context = context;
            _reactionHubContext = reactionHubContext;
        }

        [HttpPost("blogpost/{postId}/react")]
        public async Task<IActionResult> ReactToBlogPost(int postId, [FromBody] ReactionType reactionType)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var existingReaction = await _context.Reactions.FirstOrDefaultAsync(r => r.BlogPostId == postId && r.UserId == userId);

            if (existingReaction != null)
            {
                if (existingReaction.Type == reactionType)
                {
                    _context.Reactions.Remove(existingReaction);
                }
                else
                {
                    existingReaction.Type = reactionType;
                }
            }
            else
            {
                var newReaction = new Reaction
                {
                    BlogPostId = postId,
                    UserId = userId,
                    Type = reactionType,
                    CreationDate = DateTime.Now
                };
                _context.Reactions.Add(newReaction);
            }

            await _context.SaveChangesAsync();

            // Notify post author
            var postAuthorId = await GetPostAuthorId(postId);

            var post = await _context.BlogPosts.FindAsync(postId);
            if (post == null)
            {
                var notificationMessage = $"{User.Identity.Name} has reacted to your post '{postId}' with '{reactionType}' at {DateTime.Now}.";
                await _reactionHubContext.Clients.User(post.AuthorId).ReceiveReactionNotification(postId, notificationMessage);
            }

            return Ok();
        }

        [HttpPost("comment/{commentId}/react")]
        public async Task<IActionResult> ReactToComment(int commentId, [FromBody] ReactionType reactionType)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var existingReaction = await _context.Reactions.FirstOrDefaultAsync(r => r.CommentId == commentId && r.UserId == userId);

            if (existingReaction != null)
            {
                if (existingReaction.Type == reactionType)
                {
                    _context.Reactions.Remove(existingReaction);
                }
                else
                {
                    existingReaction.Type = reactionType;
                }
            }
            else
            {
                var newReaction = new Reaction
                {
                    CommentId = commentId,
                    UserId = userId,
                    Type = reactionType,
                    CreationDate = DateTime.Now
                };
                _context.Reactions.Add(newReaction);
            }

            await _context.SaveChangesAsync();

            // Notify comment author
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment != null)
            {
                var notificationMessage = $"{User.Identity.Name} has reacted to your comment '{commentId}' with '{reactionType}' at {DateTime.Now}.";
                await _reactionHubContext.Clients.User(comment.AuthorId).ReceiveReactionNotification(commentId, notificationMessage);
            }

            return Ok();
        }

        [HttpGet("blogpost/{postId}/votes")]
        public async Task<IActionResult> GetBlogPostVotes(int postId)
        {
            var upvotes = await _context.Reactions.CountAsync(r => r.BlogPostId == postId && r.Type == ReactionType.Upvote);
            var downvotes = await _context.Reactions.CountAsync(r => r.BlogPostId == postId && r.Type == ReactionType.Downvote);

            return Ok(new { Upvotes = upvotes, Downvotes = downvotes });
        }

        [HttpGet("comment/{commentId}/votes")]
        public async Task<IActionResult> GetCommentVotes(int commentId)
        {
            var upvotes = await _context.Reactions.CountAsync(r => r.CommentId == commentId && r.Type == ReactionType.Upvote);
            var downvotes = await _context.Reactions.CountAsync(r => r.CommentId == commentId && r.Type == ReactionType.Downvote);

            return Ok(new { Upvotes = upvotes, Downvotes = downvotes });
        }

        private async Task<string> GetPostAuthorId(int postId)
        {
            var post = await _context.BlogPosts.FindAsync(postId);
            return post?.AuthorId;
        }
    }
}
