using Waggle.AuthService.Logging;
using Waggle.AuthService.Saga.Context;
using Waggle.AuthService.Services;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;

namespace Waggle.AuthService.Saga.Steps
{
    public class DeleteAuthUserStep : ISagaStep<DeletionSagaContext>
    {
        private readonly IKeycloakClient _keycloakClient;
        private readonly ILogger<DeleteAuthUserStep> _logger;

        public DeleteAuthUserStep(IKeycloakClient keycloakClient, ILogger<DeleteAuthUserStep> logger)
        {
            _keycloakClient = keycloakClient;
            _logger = logger;
        }

        public async Task<Result> ExecuteAsync(DeletionSagaContext context)
        {
            var tokenResult = await _keycloakClient.GetAdminTokenAsync();
            if (!tokenResult.Success)
                return Result.Fail(tokenResult.Message, tokenResult.ErrorCode);

            var deleteResult = await _keycloakClient.DeleteUserAsync(context.Id, tokenResult.Data!);
            if (!deleteResult.Success)
            {
                _logger.LogKeycloakUserDeletionFailed(context.Id, deleteResult.Message, deleteResult.ErrorCode);
                return deleteResult;
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
