using Azure.Messaging.ServiceBus;
using MassTransit;
using System.Text.Json;

namespace Waggle.Common.Messaging
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ServiceBusClient? _serviceBusClient;
        private readonly string _queueName = "jobs";

        public EventPublisher(IPublishEndpoint publishEndpoint, string? serviceBusConnection)
        {
            _publishEndpoint = publishEndpoint;
            if (!string.IsNullOrWhiteSpace(serviceBusConnection))
                _serviceBusClient = new ServiceBusClient(serviceBusConnection);
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            await _publishEndpoint.Publish(@event, cts.Token);

            if (_serviceBusClient == null) return;
            await using var sender = _serviceBusClient.CreateSender(_queueName);
            var messageBody = JsonSerializer.Serialize(@event);

            var message = new ServiceBusMessage(messageBody)
            {
                ApplicationProperties =
                {
                    ["EventType"] = @event.GetType().Name
                }
            };

            await sender.SendMessageAsync(message);
        }
    }
}
