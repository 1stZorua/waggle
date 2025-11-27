using AutoMapper;
using MassTransit;
using Waggle.AuthService.Constants;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.Logging;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Results.Core;
using Waggle.Common.Validation;
using Waggle.Contracts.Auth.Events;
using Waggle.Contracts.User.Extensions;
using Waggle.Contracts.User.Interfaces;

namespace Waggle.AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IKeycloakClient _keycloakClient;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IUserDataClient _userDataClient;
        private readonly IServiceValidator _validator;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IKeycloakClient keycloakClient, IMapper mapper, IPublishEndpoint publishEndpoint, IUserDataClient userDataClient, IServiceValidator validator, ILogger<AuthService> logger)
        {
            _keycloakClient = keycloakClient;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _userDataClient = userDataClient;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<RegisterResponseDto>> CreateUserAsync(RegisterRequestDto request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.Success)
                return Result<RegisterResponseDto>.ValidationFail(validationResult.ValidationErrors!);

            var tokenResult = await _keycloakClient.GetAdminTokenAsync();

            if (!tokenResult.Success)
            {
                _logger.LogAdminTokenFailed(request.Username, tokenResult.Message);
                return Result<RegisterResponseDto>.Fail(tokenResult.Message, tokenResult.ErrorCode);
            }

            var adminToken = tokenResult.Data!;

            var keycloakUser = _mapper.Map<KeycloakUserRequest>(request, opt =>
            {
                opt.Items["Enabled"] = true;
                opt.Items["Credentials"] = new[] { new Credential("password", request.Password, false) };
            });

            var createResult = await _keycloakClient.CreateUserAsync(keycloakUser, adminToken);
            if (!createResult.Success)
            {
                _logger.LogKeycloakUserCreationFailed(request.Username, createResult.Message, createResult.ErrorCode);
                return Result<RegisterResponseDto>.Fail(createResult.Message, createResult.ErrorCode);
            }

            var userId = createResult.Data;

            try
            {
                var registeredEvent = _mapper.Map<RegisteredEvent>(request);
                registeredEvent.Id = userId;
                await _publishEndpoint.Publish(registeredEvent);
            }
            catch (Exception ex)
            {
                _logger.LogRegisteredEventPublishFailed(ex, request.Username, userId);
                await _keycloakClient.DeleteUserAsync(userId, adminToken);
                return Result<RegisterResponseDto>.Fail(AuthErrors.User.ProfileInitFailed, ErrorCodes.ServiceFailed);
            }

            _logger.LogUserRegistered(request.Username, userId);

            var registeredUser = _mapper.Map<RegisterResponseDto>(request);
            registeredUser.Id = userId.ToString();

            return registeredUser;
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
            if (currentUser == null) return Result.Fail(AuthErrors.User.InfoRetrievalFailed, ErrorCodes.Forbidden);

            bool isAdmin = currentUser.IsAdmin();
            bool isSelf = currentUser.IsSelf(request.Id);

            if (!isSelf && !isAdmin)
            {
                _logger.LogUnauthorizedDeleteAttempt(currentUser.GetUserId(), request.Id);
                return Result.Fail(ErrorMessages.Authentication.PermissionRequired, ErrorCodes.Forbidden);
            }

            var tokenResult = await _keycloakClient.GetAdminTokenAsync();
            if (!tokenResult.Success)
            {
                _logger.LogAdminTokenFailed(request.Id.ToString(), tokenResult.Message);
                return Result.Fail(tokenResult.Message, tokenResult.ErrorCode);
            }

            var adminToken = tokenResult.Data!;

            var keycloakResult = await _keycloakClient.DeleteUserAsync(request.Id, adminToken);
            if (!keycloakResult.Success)
            {
                _logger.LogKeycloakUserDeletionFailed(request.Id, keycloakResult.Message, keycloakResult.ErrorCode);
                return keycloakResult;
            }

            try
            {
                var deletedEvent = _mapper.Map<DeletedEvent>(request.Id);
                await _publishEndpoint.Publish(deletedEvent);
            }
            catch (Exception ex)
            {
                _logger.LogDeletedEventPublishFailed(ex, request.Id);
            }

            return Result.Ok();
        }
    }
}
