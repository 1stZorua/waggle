using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Waggle.Common.Extensions
{
    public static class LoggingExtensions
    {
        public static WebApplicationBuilder UseCommonSerilog(this WebApplicationBuilder builder)
        {
            var isDevelopment = builder.Environment.IsDevelopment();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .WriteTo.Console(isDevelopment
                    ? new CompactJsonFormatter()
                    : new RenderedCompactJsonFormatter())
                .MinimumLevel.Is(isDevelopment
                    ? LogEventLevel.Debug
                    : LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .CreateLogger();

            builder.Host.UseSerilog();

            return builder;
        }

        public static void UseSerilogOnShutdown(this WebApplication app)
        {
            app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);
        }
    }
}
