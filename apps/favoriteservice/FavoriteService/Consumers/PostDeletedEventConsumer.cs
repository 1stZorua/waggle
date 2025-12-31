using MassTransit;
using Waggle.FavoriteService.Services;
using Waggle.Contracts.Post.Events;

namespace Waggle.FavoriteService.Consumers
{
    public class PostDeletedEventConsumer : IConsumer<PostDeletedEvent>
    {
        private readonly ILogger<PostDeletedEventConsumer> _logger;
        private readonly IFavoriteService _favoriteService;

        public PostDeletedEventConsumer(ILogger<PostDeletedEventConsumer> logger, IFavoriteService favoriteService)
        {
            _logger = logger;
            _favoriteService = favoriteService;
        }

        public async Task Consume(ConsumeContext<PostDeletedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Received DeletedEvent: PostId = {Id}",
                @event.Id
            );

            await _favoriteService.HandlePostDeletedEventAsync(@event);
        }
    }
}
