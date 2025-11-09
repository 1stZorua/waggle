using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;

namespace Waggle.Common.Extensions
{
    public static class HealthCheckExtensions
    {
        public static WebApplication MapCommonHealthChecks(this WebApplication app, string pattern = "/health")
        {
            app.MapHealthChecks(pattern, new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            return app;
        }
    }
}
