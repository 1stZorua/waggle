using MassTransit;
using Waggle.Contracts.Auth.Events;
using Waggle.PostService.Services;

namespace Waggle.PostService.Consumers
{
    public class UserDeletedEventConsumer : IConsumer<UserDeletedEvent>
    {
        private readonly ILogger<UserDeletedEventConsumer> _logger;
        private readonly IPostService _postService;

        public UserDeletedEventConsumer(ILogger<UserDeletedEventConsumer> logger, IPostService postService)
        {
            _logger = logger;
            _postService = postService;
        }

        public async Task Consume(ConsumeContext<UserDeletedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Received DeletedEvent: UserId = {Id}",
                @event.Id
            );

            await _postService.HandleUserDeletedEventAsync(@event);
        }
    }
}
