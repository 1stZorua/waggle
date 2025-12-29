using MassTransit;
using Waggle.Contracts.Auth.Events;
using Waggle.UserService.Services;

namespace Waggle.UserService.Consumers
{
    public class UserDeletedEventConsumer : IConsumer<UserDeletedEvent>
    {
        private readonly ILogger<UserDeletedEventConsumer> _logger;
        private readonly IUserService _userService;

        public UserDeletedEventConsumer(ILogger<UserDeletedEventConsumer> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<UserDeletedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Received DeletedEvent: UserId = {Id}",
                @event.Id
            );

            await _userService.HandleUserDeletedEventAsync(@event);
        }
    }
}
