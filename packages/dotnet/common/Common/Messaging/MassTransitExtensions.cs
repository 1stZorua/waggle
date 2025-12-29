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
            string serviceName,
            Action<IBusRegistrationConfigurator>? configureConsumers = null)
        {
            var rabbitMqSettings = configuration
                .GetSection(RabbitMQOptions.SectionName)
                .Get<RabbitMQOptions>() ?? new RabbitMQOptions();

            var serviceBusConnection = configuration["ServiceBusConnection"];

            services.AddScoped<IEventPublisher>(sp =>
                new EventPublisher(
                    sp.GetRequiredService<IPublishEndpoint>(),
                    serviceBusConnection));

            services.AddMassTransit(x =>
            {
                configureConsumers?.Invoke(x);

                x.SetEndpointNameFormatter(
                    new KebabCaseEndpointNameFormatter(serviceName, false)
                );

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                    {
                        h.Username(rabbitMqSettings.Username);
                        h.Password(rabbitMqSettings.Password);
                        h.Heartbeat(5);
                        h.RequestedConnectionTimeout(1000);
                    });

                    cfg.UseMessageRetry(r => r.Interval(1, TimeSpan.FromSeconds(1)));
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
