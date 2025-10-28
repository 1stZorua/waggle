

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Waggle.Common.Observability
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddCommonObservability(this IServiceCollection services, string serviceName) 
        {
            services.AddOpenTelemetry()
                .ConfigureResource(r => r
                    .AddService(serviceName))
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsqlInstrumentation()
                    .AddMeter(serviceName)
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddPrometheusExporter())
                .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddNpgsql()
                    .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName));

            return services;
        }

        public static WebApplication UseCommonPrometheusEndpoint(this WebApplication app)
        {
            app.MapPrometheusScrapingEndpoint();
            return app;
        }
    }
}
