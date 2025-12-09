using Microsoft.EntityFrameworkCore;
using Waggle.MediaService.Models;

namespace Waggle.MediaService.Data
{
    public class MediaDbContext : DbContext
    {
        public MediaDbContext(DbContextOptions<MediaDbContext> opt) : base(opt)
        {
            
        }

        public DbSet<Media> Media { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Media>()
                .HasIndex(m => m.UploaderId);

            builder.Entity<Media>()
                .HasIndex(m => m.CreatedAt);

            builder.Entity<Media>()
                .HasIndex(m => m.BucketName);

            builder.Entity<Media>()
                .HasIndex(m => new { m.UploaderId, m.CreatedAt });
        }
    }
}
