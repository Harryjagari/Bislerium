using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bislerium.server.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Bislerium.server.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<CommentUpdateHistory> CommentUpdateHistories { get; set; }
        public DbSet<BlogPostUpdateHistory> BlogPostUpdateHistories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BlogPost>()
                .HasMany(bp => bp.Comments)
                .WithOne(c => c.BlogPost)
                .HasForeignKey(c => c.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlogPost>()
                .HasMany(bp => bp.Reactions)
                .WithOne(r => r.BlogPost)
                .HasForeignKey(r => r.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. If comment, Notification, Reaction of particular BlogPost is deleted then the BlogPost must not be deleted
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.BlogPost)
                .WithMany(bp => bp.Comments)
                .HasForeignKey(c => c.BlogPostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.BlogPost)
                .WithMany(bp => bp.Reactions)
                .HasForeignKey(r => r.BlogPostId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. If user is deleted then all related BlogPost, Comments, notification, reaction must be deleted.
            modelBuilder.Entity<User>()
                .HasMany(u => u.BlogPosts)
                .WithOne(bp => bp.Author)
                .HasForeignKey(bp => bp.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.Author)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Reactions)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 4. If comment, Notification, Reaction, BlogPost of particular User is deleted then the User must not be deleted
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reactions)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BlogPost>()
                .HasOne(bp => bp.Author)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(bp => bp.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<IdentityRole>().HasData
            (
                new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new IdentityRole() { Name = "Blogger", ConcurrencyStamp = "2", NormalizedName = "Blogger" }
            );
        }


    }
}
