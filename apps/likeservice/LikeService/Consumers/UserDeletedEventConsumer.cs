using MassTransit;
using Waggle.Contracts.Auth.Events;
using Waggle.LikeService.Services;

namespace Waggle.LikeService.Consumers
{
    public class UserDeletedEventConsumer : IConsumer<UserDeletedEvent>
    {
        private readonly ILogger<UserDeletedEventConsumer> _logger;
        private readonly ILikeService _likeService;

        public UserDeletedEventConsumer(ILogger<UserDeletedEventConsumer> logger, ILikeService likeService)
        {
            _logger = logger;
            _likeService = likeService;
        }

        public async Task Consume(ConsumeContext<UserDeletedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Received DeletedEvent: UserId = {Id}",
                @event.Id
            );

            await _likeService.HandleUserDeletedEventAsync(@event);
        }
    }
}
