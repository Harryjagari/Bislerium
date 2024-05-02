using Microsoft.EntityFrameworkCore;
using Bislerium.server.Data.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Bislerium.server.Data
{

    public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<BlogPost>()
                .HasOne(b => b.Author)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdentityRole>().HasData
                (
                new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new IdentityRole() { Name = "Blogger", ConcurrencyStamp = "2", NormalizedName = "Blogger" },
                 new IdentityRole() { Name = "Surfer", ConcurrencyStamp = "3", NormalizedName = "Surfer" }

                );


        }
    }

}
