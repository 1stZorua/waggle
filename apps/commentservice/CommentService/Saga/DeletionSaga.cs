using Waggle.CommentService.Saga.Context;
using Waggle.CommentService.Saga.Steps;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;

namespace Waggle.CommentService.Saga
{
    public class DeletionSaga
    {
        private readonly SagaCoordinator<DeletionSagaContext> _coordinator;

        public DeletionSaga(
            DeleteCommentStep deleteCommentStep,
            CleanupStep cleanupStep)
        {
            _coordinator = new SagaCoordinator<DeletionSagaContext>();

            _coordinator.AddStep(deleteCommentStep);
            _coordinator.AddStep(cleanupStep);
        }

        public async Task<Result> ExecuteAsync(DeletionSagaContext context)
        {
            return await _coordinator.ExecuteAsync(context);
        }
    }
}
