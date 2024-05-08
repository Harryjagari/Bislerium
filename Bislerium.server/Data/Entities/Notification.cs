using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bislerium.server.Data.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public string UserId { get; set; }

        public int BlogPostId { get; set; }

        public int CommentId { get; set; }

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
