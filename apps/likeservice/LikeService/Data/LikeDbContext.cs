using Microsoft.EntityFrameworkCore;
using Waggle.LikeService.Models;

namespace Waggle.LikeService.Data
{
    public class LikeDbContext : DbContext
    {
        public LikeDbContext(DbContextOptions<LikeDbContext> opt) : base(opt)
        {

        }

        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Like>(entity =>
            {
                entity.Property(l => l.TargetType).HasConversion<string>();

                entity.HasIndex(l => new { l.UserId, l.TargetId, l.TargetType })
                      .IsUnique();

                entity.HasIndex(l => l.UserId);
                entity.HasIndex(l => new { l.TargetId, l.TargetType });
                entity.HasIndex(l => l.CreatedAt);
            });
        }
    }
}