using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Waggle.Common.Extensions
{
    public static class PipelineExtensions
    {
        public static WebApplication UseCommonPipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            return app;
        }

        public static WebApplication MapCommonGrpcReflection(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
                app.MapGrpcReflectionService();

            return app;
        }
    }
}
