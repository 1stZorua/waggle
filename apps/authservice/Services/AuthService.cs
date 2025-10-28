using AuthService.Constants;
using AuthService.Dtos;
using AuthService.Logging;
using AuthService.Models;
using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using Waggle.Common.Constants;
using Waggle.Common.Results;
using Waggle.Contracts.Auth.Events;

namespace AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _http;
        private readonly KeycloakSettings _settings;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<AuthService> _logger;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AuthService(HttpClient http, IOptions<KeycloakSettings> opts, IMapper mapper, IPublishEndpoint publishEndpoint, ILogger<AuthService> logger)
        {
            _http = http;
            _settings = opts.Value;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<Result<RegisterResponseDto>> CreateUserAsync(RegisterRequestDto request)
        {
            var tokenResult = await PostTokenAsync(BuildForm(
                ("grant_type", "client_credentials"),
                ("client_id", _settings.AdminClientId),
                ("client_secret", _settings.AdminClientSecret)
            ));

            if (!tokenResult.Success)
            {
                _logger.LogAdminTokenFailed(request.Username, tokenResult.Message);
                return Result<RegisterResponseDto>.Fail(tokenResult.Message ?? AuthErrors.Token.AdminAccessFailed);
            }

            var token = tokenResult.Data!;

            var user = new
            {
                username = request.Username,
                email = request.Email,
                firstName = request.FirstName,
                lastName = request.LastName,
                enabled = true,
                credentials = new[]
                {
                    new { type = "password", value = request.Password, temporary = false }
                }
            };

            var json = JsonSerializer.Serialize(user);
            var requestContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var req = new HttpRequestMessage(HttpMethod.Post, $"{_settings.AuthServerUrl}/admin/realms/{_settings.Realm}/users")
            {
                Content = requestContent,
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken) }
            };

            var keycloakResult = await SendKeycloakRequestWithLocationAsync(
                req,
                AuthErrors.User.CreationFailed,
                location => location.Split('/').Last()
            );

            if (!keycloakResult.Success)
            {
                _logger.LogKeycloakUserCreationFailed(request.Username, keycloakResult.Message, keycloakResult.ErrorCode);
                return Result<RegisterResponseDto>.Fail(keycloakResult.Message ?? AuthErrors.User.CreationFailed, keycloakResult.ErrorCode);
            }

            var keycloakUserId = Guid.Parse(keycloakResult.Data!);

            var registeredEvent = _mapper.Map<RegisteredEvent>(request);
            registeredEvent.UserId = keycloakUserId;

            try
            {
                await _publishEndpoint.Publish(registeredEvent);
            }
            catch (Exception ex)
            {
                _logger.LogRegisteredEventPublishFailed(ex, request.Username, keycloakUserId);
                await DeleteKeycloakUserAsync(token.AccessToken, keycloakUserId.ToString());
                return Result<RegisterResponseDto>.Fail(AuthErrors.User.ProfileInitFailed, ErrorCodes.ServiceFailed);
            }

            _logger.LogUserRegistered(request.Username, keycloakUserId);

            return Result<RegisterResponseDto>.Ok(new()
            {
                UserId = keycloakUserId.ToString()
            });
        }

        public async Task<Result<TokenResponseDto>> PasswordGrantAsync(LoginRequestDto request)
        {
            var content = BuildForm(
                ("grant_type", "password"),
                ("client_id", _settings.ClientId),
                ("client_secret", _settings.ClientSecret),
                ("username", request.Username),
                ("password", request.Password),
                ("scope", "openid email profile")
            );

            return await PostTokenAsync(content);
        }

        public async Task<Result<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var content = BuildForm(
                ("grant_type", "refresh_token"),
                ("client_id", _settings.ClientId),
                ("client_secret", _settings.ClientSecret),
                ("refresh_token", request.RefreshToken)
            );

            return await PostTokenAsync(content);
        }

        public async Task<Result> LogoutAsync(LogoutRequestDto request)
        {
            var content = BuildForm(
                ("client_id", _settings.ClientId),
                ("client_secret", _settings.ClientSecret),
                ("refresh_token", request.RefreshToken)
            );

            var req = new HttpRequestMessage(HttpMethod.Post, GetEndpoint("logout"))
            {
                Content = content
            };
            return await SendKeycloakRequestAsync(req, AuthErrors.Session.EndFailed);
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
            return await GetUserInfoAsync(token);
        }

        private string GetEndpoint(string path) =>
            $"{_settings.AuthServerUrl}/realms/{_settings.Realm}/protocol/openid-connect/{path}";

        private static FormUrlEncodedContent BuildForm(params (string key, string value)[] pairs) =>
            new(pairs.ToDictionary(p => p.key, p => p.value));

        private async Task<Result<TokenResponseDto>> PostTokenAsync(FormUrlEncodedContent content)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, GetEndpoint("token")) { Content = content };
            var result = await SendKeycloakRequestAsync<TokenResponse>(req, AuthErrors.Token.RetrievalFailed);

            if (!result.Success)
            {
                _logger.LogTokenRetrievalFailed(result.Message);
                return Result<TokenResponseDto>.Fail(result.Message ?? AuthErrors.Token.RetrievalFailed, result.ErrorCode);
            }

            var dto = _mapper.Map<TokenResponseDto>(result.Data!);
            return Result<TokenResponseDto>.Ok(dto);
        }

        private async Task<Result<UserInfoDto>> GetUserInfoAsync(string? accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                _logger.LogMissingAccessToken();
                return Result<UserInfoDto>.Fail(AuthErrors.Token.Missing, ErrorCodes.InvalidInput);
            }

            var req = new HttpRequestMessage(HttpMethod.Get, GetEndpoint("userinfo"))
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", accessToken) }
            };

            return await SendKeycloakRequestAsync<UserInfoDto>(req, AuthErrors.User.InfoRetrievalFailed);
        }

        private async Task<Result> DeleteKeycloakUserAsync(string adminToken, string userId)
        {
            var req = new HttpRequestMessage(
                HttpMethod.Delete,
                $"{_settings.AuthServerUrl}/admin/realms/{_settings.Realm}/users/{userId}")
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", adminToken) }
            };

            return await SendKeycloakRequestAsync(req, AuthErrors.User.DeletionFailed);
        }


        private async Task<Result<T>> SendKeycloakRequestAsync<T>(HttpRequestMessage request, string failureMessage)
        {
            try
            {
                var res = await _http.SendAsync(request);
                var json = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                    return Result<T>.FromHttpStatus(res.StatusCode, failureMessage);

                var data = JsonSerializer.Deserialize<T?>(json, _jsonOptions)
                    ?? throw new JsonException();

                return Result<T>.Ok(data);
            }
            catch (JsonException ex)
            {
                _logger.LogJsonParseError(ex, request.RequestUri?.ToString());
                return Result<T>.Fail(AuthErrors.Response.ParseFailed, ErrorCodes.ServiceFailed);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogKeycloakRequestException(ex, request.RequestUri?.ToString());
                return Result<T>.Fail(AuthErrors.Service.Unavailable, ErrorCodes.ServiceUnavailable);
            }
            catch (Exception ex)
            {
                _logger.LogKeycloakRequestFailed(ex, request.RequestUri?.ToString());
                return Result<T>.Fail(AuthErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        private async Task<Result> SendKeycloakRequestAsync(HttpRequestMessage request, string failureMessage)
        {
            try
            {
                var res = await _http.SendAsync(request);

                if (!res.IsSuccessStatusCode)
                    return Result.FromHttpStatus(res.StatusCode, failureMessage);

                return Result.Ok();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogKeycloakRequestException(ex, request.RequestUri?.ToString());
                return Result.Fail(AuthErrors.Service.Unavailable, ErrorCodes.ServiceUnavailable);
            }
            catch (Exception ex)
            {
                _logger.LogKeycloakRequestFailed(ex, request.RequestUri?.ToString());
                return Result.Fail(AuthErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }
        private async Task<Result<T>> SendKeycloakRequestWithLocationAsync<T>(
            HttpRequestMessage request,
            string failureMessage,
            Func<string, T> mapLocationToResult)
        {
            try
            {
                var res = await _http.SendAsync(request);
                if (!res.IsSuccessStatusCode)
                    return Result<T>.FromHttpStatus(res.StatusCode, failureMessage);

                var location = res.Headers.Location?.ToString();
                if (string.IsNullOrEmpty(location))
                {
                    _logger.LogMissingLocationHeader(request.RequestUri?.ToString());
                    return Result<T>.Fail(AuthErrors.Response.MissingLocation, ErrorCodes.ServiceFailed);
                }

                var result = mapLocationToResult(location);
                return Result<T>.Ok(result);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogKeycloakRequestException(ex, request.RequestUri?.ToString());
                return Result<T>.Fail(AuthErrors.Service.Unavailable, ErrorCodes.ServiceUnavailable);
            }
            catch (Exception ex)
            {
                _logger.LogKeycloakRequestFailed(ex, request.RequestUri?.ToString());
                return Result<T>.Fail(AuthErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }
    }
}
