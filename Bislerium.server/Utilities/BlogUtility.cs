using System;
using System.Linq;
using Bislerium.server.Data.Entities;

namespace Bislerium.server.Utilities
{
    public static class BlogUtility
    {
        public static int CalculatePopularityScore(BlogPost blogPost)
        {
            const int upvoteWeightage = 2;
            const int downvoteWeightage = -1;
            const int commentWeightage = 1;

            int upvotesCount = blogPost.Reactions?.Count(r => r.Type == ReactionType.Upvote) ?? 0;
            int downvotesCount = blogPost.Reactions?.Count(r => r.Type == ReactionType.Downvote) ?? 0;
            int commentsCount = blogPost.Comments?.Count ?? 0;

            int popularityScore = upvoteWeightage * upvotesCount +
                                  downvoteWeightage * downvotesCount +
                                  commentWeightage * commentsCount;

            return popularityScore;
        }

    }
}
