using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bislerium.server.Data.Entities
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string AuthorId { get; set; }

        [Required]
        public Guid BlogPostId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        // Navigation properties
        [ForeignKey("AuthorId")]
        public User Author { get; set; }

        [ForeignKey("BlogPostId")]
        public BlogPost BlogPost { get; set; }

        public ICollection<Reaction> Reactions { get; set; }

        public ICollection<CommentUpdateHistory> UpdateHistories { get; set; }
    }
}
