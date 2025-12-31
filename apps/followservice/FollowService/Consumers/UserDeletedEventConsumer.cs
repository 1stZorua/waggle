using MassTransit;
using Waggle.Contracts.Auth.Events;
using Waggle.FollowService.Services;

namespace Waggle.FollowService.Consumers
{
    public class UserDeletedEventConsumer : IConsumer<UserDeletedEvent>
    {
        private readonly ILogger<UserDeletedEventConsumer> _logger;
        private readonly IFollowService _followService;

        public UserDeletedEventConsumer(ILogger<UserDeletedEventConsumer> logger, IFollowService followService)
        {
            _logger = logger;
            _followService = followService;
        }

        public async Task Consume(ConsumeContext<UserDeletedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Received DeletedEvent: UserId = {Id}",
                @event.Id
            );

            await _followService.HandleUserDeletedEventAsync(@event);
        }
    }
}
