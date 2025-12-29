using AutoMapper;
using Waggle.AuthService.Logging;
using Waggle.AuthService.Saga.Context;
using Waggle.Common.Messaging;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;
using Waggle.Contracts.Auth.Events;

namespace Waggle.AuthService.Saga.Steps
{
    public class DeleteUserStep : ISagaStep<DeletionSagaContext>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteUserStep> _logger;

        public DeleteUserStep(IEventPublisher eventPublisher, IMapper mapper, ILogger<DeleteUserStep> logger)
        {
            _eventPublisher = eventPublisher;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> ExecuteAsync(DeletionSagaContext context)
        {
            try
            {
                var deletedEvent = _mapper.Map<UserDeletedEvent>(context.Id);

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
                await _eventPublisher.PublishAsync(deletedEvent);
            }
            catch (Exception ex)
            {
                _logger.LogDeletedEventPublishFailed(ex, context.Id);
            }

            return Result.Ok();
        }

        public async Task<Result> CompensateAsync(DeletionSagaContext context)
        {
            await Task.Yield();
            return Result.Ok();
        }
    }
}
