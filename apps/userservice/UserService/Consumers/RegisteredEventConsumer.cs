using MassTransit;
using Waggle.Contracts.Auth.Events;
using Waggle.UserService.Services;

namespace Waggle.UserService.Consumers
{
    public class RegisteredEventConsumer : IConsumer<RegisteredEvent>
    {
        private readonly ILogger<RegisteredEventConsumer> _logger;
        private readonly IUserService _userService;

        public RegisteredEventConsumer(ILogger<RegisteredEventConsumer> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<RegisteredEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Received RegisteredEvent: UserId = {Id}, Email = {Email}",
                @event.Id,
                @event.Email
            );

            await _userService.HandleUserRegisteredAsync(@event);
        }
    }
}
