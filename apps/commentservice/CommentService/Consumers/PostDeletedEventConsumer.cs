using MassTransit;
using Waggle.CommentService.Services;
using Waggle.Contracts.Post.Events;

namespace Waggle.CommentService.Consumers
{
    public class PostDeletedEventConsumer : IConsumer<PostDeletedEvent>
    {
        private readonly ILogger<PostDeletedEventConsumer> _logger;
        private readonly ICommentService _commentService;

        public PostDeletedEventConsumer(ILogger<PostDeletedEventConsumer> logger, ICommentService commentService)
        {
            _logger = logger;
            _commentService = commentService;
        }

        public async Task Consume(ConsumeContext<PostDeletedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Received DeletedEvent: PostId = {Id}",
                @event.Id
            );

            await _commentService.HandlePostDeletedEventAsync(@event);
        }
    }
}
