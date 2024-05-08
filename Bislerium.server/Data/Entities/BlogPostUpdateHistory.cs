using System.ComponentModel.DataAnnotations;

namespace Bislerium.server.Data.Entities
{
    public class BlogPostUpdateHistory
    {
        [Key]
        public Guid Id { get; set; }

        public Guid BlogPostId { get; set; }

        public string OriginalTitle { get; set; }
        public string UpdatedTitle { get; set; }

        public string OriginalBody { get; set; }
        public string UpdatedBody { get; set; }

        public string OriginalImageUrl { get; set; }
        public string UpdatedImageUrl { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
