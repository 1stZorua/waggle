using Microsoft.EntityFrameworkCore;
using Waggle.UserService.Models;

namespace Waggle.UserService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<UserDbContext>();

            try
            {
                context.Database.Migrate();
            } catch (Exception)
            {
                throw;
            }

            SeedData(context);
        }

        private static void SeedData(UserDbContext context)
        {
            if (context.Users.Any()) return;

            context.Users.AddRange(
                new User()
                {
                    Username = "John",
                    Email = "john@gmail.com",
                    FirstName = "John",
                    LastName = "Doe"
                }
            );

            context.SaveChanges();
        }
    }
}
