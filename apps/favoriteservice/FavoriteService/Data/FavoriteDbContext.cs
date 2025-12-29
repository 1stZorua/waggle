using Microsoft.EntityFrameworkCore;
using Waggle.FavoriteService.Models;

namespace Waggle.FavoriteService.Data
{
    public class FavoriteDbContext : DbContext
    {
        public FavoriteDbContext(DbContextOptions<FavoriteDbContext> opt) : base(opt)
        {
        }

        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Favorite>()
                .HasIndex(f => new { f.UserId, f.TargetId, f.TargetType })
                .IsUnique();

            builder.Entity<Favorite>()
                .HasIndex(f => f.UserId);

            builder.Entity<Favorite>()
                .HasIndex(f => new { f.TargetId, f.TargetType });

            builder.Entity<Favorite>()
                .HasIndex(f => f.CreatedAt);

            builder.Entity<Favorite>()
                .Property(f => f.TargetType)
                .HasConversion<string>();
        }
    }
}