using MassTransit;
using Waggle.Contracts.Comment.Events;
using Waggle.LikeService.Services;

namespace Waggle.LikeService.Consumers
{
    public class CommentDeletedEventConsumer : IConsumer<CommentDeletedEvent>
    {
        private readonly ILogger<CommentDeletedEventConsumer> _logger;
        private readonly ILikeService _likeService;

        public CommentDeletedEventConsumer(ILogger<CommentDeletedEventConsumer> logger, ILikeService likeService)
        {
            _logger = logger;
            _likeService = likeService;
        }

        public async Task Consume(ConsumeContext<CommentDeletedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Received DeletedEvent: CommentId = {Id}",
                @event.Id
            );

            await _likeService.HandleCommentDeletedEventAsync(@event);
        }
    }
}
