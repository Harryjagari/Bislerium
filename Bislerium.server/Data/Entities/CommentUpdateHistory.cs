using System.ComponentModel.DataAnnotations;

namespace Bislerium.server.Data.Entities
{
    public class CommentUpdateHistory
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CommentId { get; set; }

        public string OriginalContent { get; set; }
        public string UpdatedContent { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
