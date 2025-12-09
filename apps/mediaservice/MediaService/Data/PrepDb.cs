using Microsoft.EntityFrameworkCore;

namespace Waggle.MediaService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<MediaDbContext>();

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
