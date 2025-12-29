using MassTransit;
using Waggle.Contracts.Post.Events;
using Waggle.MediaService.Services;

namespace Waggle.MediaService.Consumers
{
    public class PostDeletedEventConsumer : IConsumer<PostDeletedEvent>
    {
        private readonly ILogger<PostDeletedEventConsumer> _logger;
        private readonly IMediaService _mediaService;

        public PostDeletedEventConsumer(ILogger<PostDeletedEventConsumer> logger, IMediaService mediaService)
        {
            _logger = logger;
            _mediaService = mediaService;
        }

        public async Task Consume(ConsumeContext<PostDeletedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Received DeletedEvent: PostId = {Id}",
                @event.Id
            );

            await _mediaService.HandlePostDeletedEventAsync(@event);
        }
    }
}
