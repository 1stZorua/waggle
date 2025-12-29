using Microsoft.EntityFrameworkCore;

namespace Waggle.FavoriteService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<FavoriteDbContext>();

            try
            {
                context.Database.Migrate();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
