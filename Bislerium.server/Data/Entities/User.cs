using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;


namespace Bislerium.server.Data.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ? ProfilePictureUrl { get; set; }
        public DateTime RegistrationDate { get; set; }

        // Navigation properties
        public ICollection<BlogPost> BlogPosts { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Reaction> Reactions { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}

