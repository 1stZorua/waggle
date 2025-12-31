using MassTransit;
using Waggle.Contracts.Auth.Events;
using Waggle.FavoriteService.Services;

namespace Waggle.FavoriteService.Consumers
{
    public class UserDeletedEventConsumer : IConsumer<UserDeletedEvent>
    {
        private readonly ILogger<UserDeletedEventConsumer> _logger;
        private readonly IFavoriteService _favoriteService;

        public UserDeletedEventConsumer(ILogger<UserDeletedEventConsumer> logger, IFavoriteService favoriteService)
        {
            _logger = logger;
            _favoriteService = favoriteService;
        }

        public async Task Consume(ConsumeContext<UserDeletedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Received DeletedEvent: UserId = {Id}",
                @event.Id
            );

            await _favoriteService.HandleUserDeletedEventAsync(@event);
        }
    }
}
