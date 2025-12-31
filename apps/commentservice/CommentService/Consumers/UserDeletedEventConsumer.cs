using MassTransit;
using Waggle.Contracts.Auth.Events;
using Waggle.CommentService.Services;

namespace Waggle.CommentService.Consumers
{
    public class UserDeletedEventConsumer : IConsumer<UserDeletedEvent>
    {
        private readonly ILogger<UserDeletedEventConsumer> _logger;
        private readonly ICommentService _commentService;

        public UserDeletedEventConsumer(ILogger<UserDeletedEventConsumer> logger, ICommentService commentService)
        {
            _logger = logger;
            _commentService = commentService;
        }

        public async Task Consume(ConsumeContext<UserDeletedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Received DeletedEvent: UserId = {Id}",
                @event.Id
            );

            await _commentService.HandleUserDeletedEventAsync(@event);
        }
    }
}
