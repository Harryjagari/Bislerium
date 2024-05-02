namespace Bislerium.server.Data.Entities
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImageUrl { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ? LastModifiedDate { get; set; }

        // Navigation properties
        public User Author { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Reaction> Reactions { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}

