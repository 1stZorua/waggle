using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Waggle.Contracts.Auth.Events;
using Waggle.NotificationWorker.Services;

namespace Waggle.NotificationWorker.Workers
{
    public class NotificationWorker
    {
        private readonly ILogger<NotificationWorker> _logger;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;

        private readonly Dictionary<string, Func<string, Task>> _handlers;

        public NotificationWorker(ILogger<NotificationWorker> logger, IEmailService emailService, IEmailTemplateService templateService)
        {
            _logger = logger;
            _emailService = emailService;
            _templateService = templateService;

            _handlers = new Dictionary<string, Func<string, Task>>
        {
            { nameof(RegistrationCompletedEvent), HandleRegistrationCompletedEventAsync },
        };
        }

        [Function("HandleJobEvent")]
        public async Task Run(
            [ServiceBusTrigger("jobs", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            var messageBody = message.Body.ToString();
            var eventType = message.ApplicationProperties.TryGetValue("EventType", out var typeObj)
                            ? typeObj.ToString()
                            : null;

            _logger.LogInformation("Received message with EventType: '{eventType}', Body: {body}", eventType, messageBody);

            if (eventType != null && _handlers.TryGetValue(eventType, out var handler))
            {
                try
                {
                    await handler(messageBody);
                    await messageActions.CompleteMessageAsync(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing event {eventType}: {body}", eventType, messageBody);
                    await messageActions.DeadLetterMessageAsync(
                        message,
                        deadLetterReason: "ProcessingFailed",
                        deadLetterErrorDescription: ex.Message
                    );
                }
            }
            else
            {
                _logger.LogDebug("Skipping unhandled event type: {eventType}", eventType);
                await messageActions.CompleteMessageAsync(message);
            }
        }

        private async Task HandleRegistrationCompletedEventAsync(string json)
        {
            var @event = JsonSerializer.Deserialize<RegisteredEvent>(json);
            if (@event == null) return;

            _logger.LogInformation("Processing RegisteredEvent for {email}", @event.Email);

            var htmlBody = await _templateService.RenderTemplateAsync(
                "Registration.html",
                new Dictionary<string, string>
                {
                    ["name"] = @event.Username ?? "there",
                    ["appUrl"] = "https://web.waggle.local"
                }
            );

            await _emailService.SendEmailAsync(@event.Email, "Welcome to Waggle", htmlBody, true);
        }
    }
}