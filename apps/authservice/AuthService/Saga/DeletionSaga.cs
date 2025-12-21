using Waggle.AuthService.Saga.Context;
using Waggle.AuthService.Saga.Steps;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;

namespace Waggle.AuthService.Saga
{
    public class DeletionSaga
    {
        private readonly SagaCoordinator<DeletionSagaContext> _coordinator;

        public DeletionSaga(
            DeleteAuthUserStep deleteAuthUserStep,
            DeleteUserStep deleteUserStep)
        {
            _coordinator = new SagaCoordinator<DeletionSagaContext>();

            _coordinator.AddStep(deleteAuthUserStep);
            _coordinator.AddStep(deleteUserStep);
        }

        public async Task<Result> ExecuteAsync(DeletionSagaContext context)
        {
            return await _coordinator.ExecuteAsync(context);
        }
    }
}
