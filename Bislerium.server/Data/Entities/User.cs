using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bislerium.server.Data.Entities
{
    public class User : IdentityUser
    {
        // Remove unique constraint on UserName
        public override string UserName { get; set; }
        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string? ResetPasswordOTP { get; set; }
        public DateTime? ResetPasswordOTPIssueTime { get; set; }

        // Navigation properties
        [InverseProperty("Author")]
        public ICollection<BlogPost> BlogPosts { get; set; }

        [InverseProperty("Author")]
        public ICollection<Comment> Comments { get; set; }

        [InverseProperty("User")]
        public ICollection<Reaction> Reactions { get; set; }

    }
}
