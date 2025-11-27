using Microsoft.EntityFrameworkCore;
using Waggle.UserService.Models;

namespace Waggle.UserService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> opt) : base(opt)
        {
            
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
