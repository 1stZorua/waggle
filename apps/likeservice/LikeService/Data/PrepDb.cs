using Microsoft.EntityFrameworkCore;

namespace Waggle.LikeService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<LikeDbContext>();

            try
            {
                context.Database.Migrate();
            } catch (Exception)
            {
                throw;
            }
        }
    }
}
