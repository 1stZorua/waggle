using MassTransit;
using Waggle.Contracts.Auth.Events;
using Waggle.MediaService.Services;

namespace Waggle.MediaService.Consumers
{
    public class UserDeletedEventConsumer : IConsumer<UserDeletedEvent>
    {
        private readonly ILogger<UserDeletedEventConsumer> _logger;
        private readonly IMediaService _mediaService;

        public UserDeletedEventConsumer(ILogger<UserDeletedEventConsumer> logger, IMediaService mediaService)
        { 
            _logger = logger;
            _mediaService = mediaService;
        }

        public async Task Consume(ConsumeContext<UserDeletedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Received DeletedEvent: UserId = {Id}",
                @event.Id
            );

            await _mediaService.HandleUserDeletedEventAsync(@event);
        }
    }
}
