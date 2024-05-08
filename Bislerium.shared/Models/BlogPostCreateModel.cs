using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Bislerium.shared.Models
{

    public class BlogPostCreateModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public IFormFile Image { get; set; }
    }
}

