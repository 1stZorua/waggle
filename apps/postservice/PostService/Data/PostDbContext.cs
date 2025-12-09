using Microsoft.EntityFrameworkCore;
using Waggle.PostService.Models;

namespace Waggle.PostService.Data
{
    public class PostDbContext : DbContext
    {
        public PostDbContext(DbContextOptions<PostDbContext> opt) : base(opt)
        {
            
        }

        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Post>()
                .HasIndex(p => p.UserId);

            builder.Entity<Post>()
                .HasIndex(p => p.CreatedAt);
        }
    }
}
