using MassTransit;
using Waggle.Contracts.Auth.Events;
using Waggle.UserService.Services;

namespace Waggle.UserService.Consumers
{
    public class DeletedEventConsumer : IConsumer<DeletedEvent>
    {
        private readonly ILogger<DeletedEventConsumer> _logger;
        private readonly IUserService _userService;

        public DeletedEventConsumer(ILogger<DeletedEventConsumer> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<DeletedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Received DeletedEvent: UserId = {Id}",
                @event.Id
            );

            await _userService.HandleUserDeletedAsync(@event);
        }
    }
}
