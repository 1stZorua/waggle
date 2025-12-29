using AutoMapper;
using Waggle.PostService.Logging;
using Waggle.PostService.Saga.Context;
using Waggle.Common.Messaging;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;
using Waggle.Contracts.Post.Events;
using Waggle.PostService.Data;
using Waggle.PostService.Constants;
using Waggle.Common.Constants;

namespace Waggle.PostService.Saga.Steps
{
    public class DeleteMediaStep : ISagaStep<DeletionSagaContext>
    {
        private readonly IPostRepository _repo;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteMediaStep> _logger;

        public DeleteMediaStep(IPostRepository repo, IEventPublisher eventPublisher, IMapper mapper, ILogger<DeleteMediaStep> logger)
        {
            _repo = repo;
            _eventPublisher = eventPublisher;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> ExecuteAsync(DeletionSagaContext context)
        {
            try
            {
                var existing = await _repo.GetPostByIdAsync(context.Id);
                if (existing == null)
                {
                    _logger.LogPostNotFound(context.Id);
                    return Result.Fail(PostErrors.Post.NotFound, ErrorCodes.NotFound);
                }

                var mediaIds = new[] { existing.ThumbnailId }.Concat(existing.MediaIds ?? []);

                var deletedEvent = _mapper.Map<PostDeletedEvent>(context);
                deletedEvent.MediaIds = mediaIds;

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
