using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> opt) : base(opt)
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
}
