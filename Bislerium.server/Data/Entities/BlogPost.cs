using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bislerium.server.Data.Entities
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public string AuthorId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        // Navigation properties
        [ForeignKey("AuthorId")]
        public User Author { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Reaction> Reactions { get; set; }

        public ICollection<Notification> Notifications { get; set; }
    }
}
