using MassTransit;
using Waggle.Common.Messaging;

namespace Waggle.AuthService.IntegrationTests.Infrastructure
{
    public class MassTransitEventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
        {
            return _publishEndpoint.Publish(@event);
        }
    }
}
