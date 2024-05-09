using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Bislerium.shared.Models
{
    public class BlogPostUpdateModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public IFormFile Image { get; set; } // New property for the image file
    }
}
