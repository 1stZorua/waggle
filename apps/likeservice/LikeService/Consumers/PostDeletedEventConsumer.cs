using MassTransit;
using Waggle.Contracts.Post.Events;
using Waggle.LikeService.Services;

namespace Waggle.LikeService.Consumers
{
    public class PostDeletedEventConsumer : IConsumer<PostDeletedEvent>
    {
        private readonly ILogger<PostDeletedEventConsumer> _logger;
        private readonly ILikeService _likeService;

        public PostDeletedEventConsumer(ILogger<PostDeletedEventConsumer> logger, ILikeService likeService)
        {
            _logger = logger;
            _likeService = likeService;
        }

        public async Task Consume(ConsumeContext<PostDeletedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Received DeletedEvent: PostId = {Id}",
                @event.Id
            );

            await _likeService.HandlePostDeletedEventAsync(@event);
        }
    }
}
