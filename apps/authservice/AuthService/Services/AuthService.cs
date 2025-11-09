using Waggle.AuthService.Constants;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.Logging;
using Waggle.Common.Constants;
using Waggle.Common.Results.Core;
using Waggle.Contracts.Auth.Events;
using MassTransit;
using AutoMapper;

namespace Waggle.AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IKeycloakClient _keycloakClient;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IKeycloakClient keycloakClient, IMapper mapper, IPublishEndpoint publishEndpoint, ILogger<AuthService> logger)
        {
            _keycloakClient = keycloakClient;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<Result<RegisterResponseDto>> CreateUserAsync(RegisterRequestDto request)
        {
            var tokenResult = await _keycloakClient.GetAdminTokenAsync();

            if (!tokenResult.Success)
            {
                _logger.LogAdminTokenFailed(request.Username, tokenResult.Message);
                return Result<RegisterResponseDto>.Fail(tokenResult.Message ?? AuthErrors.Token.AdminAccessFailed, tokenResult.ErrorCode);
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
                return Result<RegisterResponseDto>.Fail(createResult.Message ?? AuthErrors.User.CreationFailed, createResult.ErrorCode);
            }

            var userId = createResult.Data;

            var registeredEvent = _mapper.Map<RegisteredEvent>(request);
            registeredEvent.UserId = userId;

            try
            {
                await _publishEndpoint.Publish(registeredEvent);
            }
            catch (Exception ex)
            {
                _logger.LogRegisteredEventPublishFailed(ex, request.Username, userId);
                await _keycloakClient.DeleteUserAsync(userId, adminToken);
                return Result<RegisterResponseDto>.Fail(AuthErrors.User.ProfileInitFailed, ErrorCodes.ServiceFailed);
            }

            _logger.LogUserRegistered(request.Username, userId);

            return Result<RegisterResponseDto>.Ok(new()
            {
                UserId = userId.ToString()
            });
        }

        public async Task<Result<TokenResponseDto>> PasswordGrantAsync(LoginRequestDto request)
        {
            var result = await _keycloakClient.GetPasswordTokenAsync(request.Username, request.Password);
            
            if (!result.Success)
            {
                _logger.LogTokenRetrievalFailed(result.Message);
                return Result<TokenResponseDto>.Fail(result.Message ?? AuthErrors.Token.RetrievalFailed, result.ErrorCode);
            }

            return _mapper.Map<TokenResponseDto>(result.Data);
        }

        public async Task<Result<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var result = await _keycloakClient.RefreshTokenAsync(request.RefreshToken);

            if (!result.Success)
            {
                _logger.LogTokenRetrievalFailed(result.Message);
                return Result<TokenResponseDto>.Fail(result.Message ?? AuthErrors.Token.RetrievalFailed, result.ErrorCode);
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
    }
}
