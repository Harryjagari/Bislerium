namespace Bislerium.server.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public int BlogPostId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        // Navigation properties
        public User Author { get; set; }
        public BlogPost BlogPost { get; set; }
        public ICollection<Reaction> Reactions { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}

