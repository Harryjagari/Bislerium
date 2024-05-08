using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bislerium.server.Data.Entities
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string AuthorId { get; set; }

        [Required]
        public int BlogPostId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        // Navigation properties
        [ForeignKey("AuthorId")]
        public User Author { get; set; }

        [ForeignKey("BlogPostId")]
        public BlogPost BlogPost { get; set; }

        public ICollection<Reaction> Reactions { get; set; }

        public ICollection<Notification> Notifications { get; set; }
    }
}
