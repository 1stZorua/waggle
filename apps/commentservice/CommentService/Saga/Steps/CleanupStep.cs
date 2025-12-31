using AutoMapper;
using Waggle.CommentService.Logging;
using Waggle.CommentService.Saga.Context;
using Waggle.Common.Messaging;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;
using Waggle.Contracts.Comment.Events;

namespace Waggle.CommentService.Saga.Steps
{
    public class CleanupStep : ISagaStep<DeletionSagaContext>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IMapper _mapper;
        private readonly ILogger<CleanupStep> _logger;

        public CleanupStep(IEventPublisher eventPublisher, IMapper mapper, ILogger<CleanupStep> logger)
        {
            _eventPublisher = eventPublisher;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> ExecuteAsync(DeletionSagaContext context)
        {
            try
            {
                var publishTasks = context.DeletedCommentIds.Select(commentId =>
                {
                    var deletedEvent = new CommentDeletedEvent
                    {
                        Id = commentId
                    };
                    return _eventPublisher.PublishAsync(deletedEvent);
                });

                await Task.WhenAll(publishTasks);
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