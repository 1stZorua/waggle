using Waggle.AuthService.Dtos;
using Waggle.AuthService.Saga.Context;
using Waggle.AuthService.Saga.Steps;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;

namespace Waggle.AuthService.Saga
{
    public class RegistrationSaga
    {
        private readonly SagaCoordinator<RegistrationSagaContext, RegisterResponseDto> _coordinator;

        public RegistrationSaga(
            CreateUserStep createUserStep,
            CreateProfileStep createProfileStep,
            NotifyUserRegisteredStep notifyUserRegisteredStep)
        {
            _coordinator = new SagaCoordinator<RegistrationSagaContext, RegisterResponseDto>();

            _coordinator.AddStep(createUserStep);
            _coordinator.AddStep(createProfileStep);
            _coordinator.AddStep(notifyUserRegisteredStep);
        }

        public async Task<Result<RegisterResponseDto>> ExecuteAsync(RegistrationSagaContext context)
        {
            return await _coordinator.ExecuteAsync(context);
        }
    }
}
