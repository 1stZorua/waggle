using Microsoft.EntityFrameworkCore;
using Waggle.FollowService.Models;

namespace Waggle.FollowService.Data
{
    public class FollowDbContext : DbContext
    {
        public FollowDbContext(DbContextOptions<FollowDbContext> opt) : base(opt)
        {
        }

        public DbSet<Follow> Follows { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Follow>()
                .HasIndex(f => new { f.FollowerId, f.FollowingId })
                .IsUnique();

            builder.Entity<Follow>()
                .HasIndex(f => f.FollowerId);

            builder.Entity<Follow>()
                .HasIndex(f => f.FollowingId);

            builder.Entity<Follow>()
                .HasIndex(f => f.CreatedAt);
        }
    }
}