using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Waggle.Common.Messaging
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMessaging(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IBusRegistrationConfigurator>? configureConsumers = null)
        {
            var rabbitMqSettings = configuration
                .GetSection(RabbitMQOptions.SectionName)
                .Get<RabbitMQOptions>() ?? new RabbitMQOptions();

            services.AddMassTransit(x =>
            {
                configureConsumers?.Invoke(x);

                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                    {
                        h.Username(rabbitMqSettings.Username);
                        h.Password(rabbitMqSettings.Password);
                    });

                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
