using UserService.Models;

namespace UserService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            SeedData(serviceScope.ServiceProvider.GetRequiredService<UserDbContext>());
        }

        private static void SeedData(UserDbContext context)
        {
            if (context.Users.Any()) return;

            context.Users.AddRange(
                new User()
                {
                    Name = "John Doe",
                    Username = "John"
                }
            );

            context.SaveChanges();
        }
    }
}
