using AutoMapper;
using Waggle.AuthService.Constants;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.Logging;
using Waggle.AuthService.Saga.Context;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;
using Waggle.Common.Validation;
using Waggle.Contracts.User.Extensions;
using Waggle.Contracts.User.Interfaces;

namespace Waggle.AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IKeycloakClient _keycloakClient;
        private readonly IMapper _mapper;
        private readonly IUserDataClient _userDataClient;
        private readonly IServiceValidator _validator;
        private readonly ISagaCoordinator<RegistrationSagaContext, RegisterResponseDto> _registrationSaga;
        private readonly ISagaCoordinator<DeletionSagaContext> _deletionSaga;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IKeycloakClient keycloakClient, 
            IMapper mapper,
            IUserDataClient userDataClient, 
            IServiceValidator validator, 
            ISagaCoordinator<RegistrationSagaContext, RegisterResponseDto> registrationSaga, 
            ISagaCoordinator<DeletionSagaContext> deletionSaga, 
            ILogger<AuthService> logger)
        {
            _keycloakClient = keycloakClient;
            _mapper = mapper;
            _userDataClient = userDataClient;
            _registrationSaga = registrationSaga;
            _deletionSaga = deletionSaga;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<RegisterResponseDto>> CreateUserAsync(RegisterRequestDto request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.Success)
                return Result<RegisterResponseDto>.ValidationFail(validationResult.ValidationErrors!);

            var sagaContext = _mapper.Map<RegistrationSagaContext>(request);
            return await _registrationSaga.ExecuteAsync(sagaContext);
        }

        public async Task<Result<TokenResponseDto>> PasswordGrantAsync(LoginRequestDto request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.Success)
                return Result<TokenResponseDto>.ValidationFail(validationResult.ValidationErrors!);
            
            var tokenResult = await _keycloakClient.GetPasswordTokenAsync(request.Identifier, request.Password);
            
            if (!tokenResult.Success)
            {
                _logger.LogTokenRetrievalFailed(tokenResult.Message);
                return Result<TokenResponseDto>.Fail(tokenResult.Message, tokenResult.ErrorCode);
            }

            var userInfoResult = await _keycloakClient.GetUserInfoAsync(tokenResult.Data!.AccessToken);

            if (!userInfoResult.Success)
            {
                _logger.LogUserInfoRetrievalFailed(request.Identifier, userInfoResult.ErrorCode);
                return Result<TokenResponseDto>.Fail(userInfoResult.Message, userInfoResult.ErrorCode);
            }

            var userId = Guid.Parse(userInfoResult.Data!.Sub);
            var userExistsResult = await _userDataClient.GetUserByIdAsync(userId);

            if (!userExistsResult.Success)
            {
                _logger.LogUserNotFoundInUserService(userId);
                return Result<TokenResponseDto>.Fail(AuthErrors.Token.InvalidCredentials, ErrorCodes.Unauthorized);
            }

            return _mapper.Map<TokenResponseDto>(tokenResult.Data);
        }

        public async Task<Result<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var result = await _keycloakClient.RefreshTokenAsync(request.RefreshToken);

            if (!result.Success)
            {
                _logger.LogTokenRetrievalFailed(result.Message);
                return Result<TokenResponseDto>.Fail(result.Message, result.ErrorCode);
            }

            return _mapper.Map<TokenResponseDto>(result.Data);
        }

        public async Task<Result> LogoutAsync(LogoutRequestDto request)
        {
            return await _keycloakClient.RevokeTokenAsync(request.RefreshToken);
        }

        public async Task<Result<UserInfoDto>> ValidateAsync(ValidateTokenRequestDto request)
        {
            var bearerToken = request.BearerToken;
            if (string.IsNullOrWhiteSpace(bearerToken) || !bearerToken.StartsWith("Bearer "))
            {
                _logger.LogInvalidBearerTokenFormat();
                return Result<UserInfoDto>.Fail(AuthErrors.Token.InvalidFormat, ErrorCodes.InvalidInput);
            }

            var token = bearerToken["Bearer ".Length..].Trim();
            return await _keycloakClient.GetUserInfoAsync(token);
        }

        public async Task<Result> DeleteUserAsync(DeleteUserRequestDto request, UserInfoDto currentUser)
        {
            if (!currentUser.TryEnsure(out var failedResult))
                return failedResult;

            bool isAdmin = currentUser.IsAdmin();
            bool isSelf = currentUser.IsSelf(request.Id);

            if (!isSelf && !isAdmin)
            {
                _logger.LogUnauthorizedDeleteAttempt(currentUser.GetUserId(), request.Id);
                return Result.Fail(ErrorMessages.Authentication.PermissionRequired, ErrorCodes.Forbidden);
            }

            var sagaContext = _mapper.Map<DeletionSagaContext>(request);
            return await _deletionSaga.ExecuteAsync(sagaContext);
        }
    }
}
