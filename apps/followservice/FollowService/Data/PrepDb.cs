using Microsoft.EntityFrameworkCore;

namespace Waggle.FollowService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<FollowDbContext>();

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
