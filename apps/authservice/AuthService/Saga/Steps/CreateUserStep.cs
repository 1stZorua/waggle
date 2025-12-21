using AutoMapper;
using Waggle.AuthService.Constants;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.Logging;
using Waggle.AuthService.Saga.Context;
using Waggle.AuthService.Services;
using Waggle.Common.Constants;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;

namespace Waggle.AuthService.Saga.Steps
{
    public class CreateUserStep : ISagaStep<RegistrationSagaContext, RegisterResponseDto>
    {
        private readonly IKeycloakClient _keycloakClient;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateUserStep> _logger;

        public CreateUserStep(IKeycloakClient keycloakClient, IMapper mapper, ILogger<CreateUserStep> logger)
        {
            _keycloakClient = keycloakClient;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<RegisterResponseDto>> ExecuteAsync(RegistrationSagaContext context)
        {
            var tokenResult = await _keycloakClient.GetAdminTokenAsync();
            if (!tokenResult.Success)
            {
                _logger.LogAdminTokenFailed(context.Username, tokenResult.Message);
                return Result<RegisterResponseDto>.Fail(tokenResult.Message, tokenResult.ErrorCode);
            }

            var adminToken = tokenResult.Data!;

            var keycloakUser = _mapper.Map<KeycloakUserRequest>(context, opt =>
            {
                opt.Items["Enabled"] = true;
                opt.Items["Credentials"] = new[] { new Credential("password", context.Password, false) };
            });

            var createResult = await _keycloakClient.CreateUserAsync(keycloakUser, adminToken);
            if (!createResult.Success)
            {
                _logger.LogKeycloakUserCreationFailed(context.Username, createResult.Message, createResult.ErrorCode);
                return Result<RegisterResponseDto>.Fail(createResult.Message, createResult.ErrorCode);
            }

            context.Id = createResult.Data;
            var response = _mapper.Map<RegisterResponseDto>(context);

            return Result<RegisterResponseDto>.Ok(response);
        }

        public async Task<Result<RegisterResponseDto>> CompensateAsync(RegistrationSagaContext context)
        {
            var tokenResult = await _keycloakClient.GetAdminTokenAsync();
            if (!tokenResult.Success)
                return Result<RegisterResponseDto>.Fail(tokenResult.Message, tokenResult.ErrorCode);

            var adminToken = tokenResult.Data!;
            await _keycloakClient.DeleteUserAsync(context.Id, adminToken);

            return Result<RegisterResponseDto>.Fail(AuthErrors.User.ProfileInitFailed, ErrorCodes.ServiceFailed);
        }
    }
}
