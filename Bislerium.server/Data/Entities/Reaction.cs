using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bislerium.server.Data.Entities
{
    public class Reaction
    {
        public Guid Id { get; set; }

        public ReactionType Type { get; set; }

        public string UserId { get; set; }

        public Guid ? BlogPostId { get; set; }

        public Guid ? CommentId { get; set; }

        public DateTime CreationDate { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("BlogPostId")]
        public BlogPost BlogPost { get; set; }

        [ForeignKey("CommentId")]
        public Comment Comment { get; set; }
    }
}
