using Microsoft.EntityFrameworkCore;
using Waggle.CommentService.Models;

namespace Waggle.CommentService.Data
{
    public class CommentDbContext : DbContext
    {
        public CommentDbContext(DbContextOptions<CommentDbContext> opt) : base(opt)
        {

        }

        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Comment>()
                .HasIndex(c => c.PostId);

            builder.Entity<Comment>()
                .HasIndex(c => c.ParentId);

            builder.Entity<Comment>()
                .HasIndex(c => c.UserId);

            builder.Entity<Comment>()
                .HasIndex(c => c.CreatedAt);

            builder.Entity<Comment>()
                .HasIndex(c => new { c.PostId, c.ParentId });
        }
    }
}