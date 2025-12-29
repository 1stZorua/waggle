using Waggle.PostService.Saga.Context;
using Waggle.PostService.Saga.Steps;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;

namespace Waggle.PostService.Saga
{
    public class DeletionSaga
    {
        private readonly SagaCoordinator<DeletionSagaContext> _coordinator;

        public DeletionSaga(
            DeleteMediaStep deleteMediaStep,
            DeletePostStep deletePostStep)
        {
            _coordinator = new SagaCoordinator<DeletionSagaContext>();

            _coordinator.AddStep(deleteMediaStep);
            _coordinator.AddStep(deletePostStep);
        }

        public async Task<Result> ExecuteAsync(DeletionSagaContext context)
        {
            return await _coordinator.ExecuteAsync(context);
        }
    }
}
