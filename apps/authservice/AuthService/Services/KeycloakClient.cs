using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using Waggle.AuthService.Constants;
using Waggle.AuthService.Logging;
using Waggle.AuthService.Models;
using Waggle.AuthService.Options;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Results.Core;

namespace Waggle.AuthService.Services
{
    public class KeycloakClient : IKeycloakClient
    {
        private readonly HttpClient _http;
        private readonly KeycloakOptions _settings;
        private readonly ILogger<KeycloakClient> _logger;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public KeycloakClient(HttpClient http, IOptions<KeycloakOptions> opts, ILogger<KeycloakClient> logger)
        {
            _http = http;
            _settings = opts.Value;
            _logger = logger;
        }

        public async Task<Result<string>> GetAdminTokenAsync()
        {
            var content = BuildForm(
                ("grant_type", "client_credentials"),
                ("client_id", _settings.AdminClientId),
                ("client_secret", _settings.AdminClientSecret)
            );

            var result = await PostTokenAsync(content);
            return result.Match(
                onSuccess: data => Result<string>.Ok(data.AccessToken),
                onFailure: _ => Result<string>.Fail(result.Message, result.ErrorCode)
            );
        }

        public async Task<Result<Guid>> CreateUserAsync(KeycloakUserRequest user, string adminToken)
        {
            var keycloakUser = new
            {
                username = user.Username,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                enabled = true,
                credentials = new[]
                {
                    new { type = "password", value = user.Password, temporary = false }
                }
            };

            var json = JsonSerializer.Serialize(keycloakUser);
            var requestContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var req = new HttpRequestMessage(HttpMethod.Post, $"{_settings.AuthServerUrl}/admin/realms/{_settings.Realm}/users")
            {
                Content = requestContent,
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", adminToken) }
            };

            var result = await SendKeycloakRequestWithLocationAsync(
                req,
                AuthErrors.User.CreationFailed,
                location => Guid.Parse(location.Split('/').Last())
            );

            return result;
        }

        public async Task<Result> DeleteUserAsync(Guid userId, string adminToken)
        {
            var req = new HttpRequestMessage(
                HttpMethod.Delete,
                $"{_settings.AuthServerUrl}/admin/realms/{_settings.Realm}/users/{userId}")
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", adminToken) }
            };

            return await SendKeycloakRequestAsync(req, AuthErrors.User.DeletionFailed);
        }

        public async Task<Result<TokenResponse>> GetPasswordTokenAsync(string username, string password)
        {
            var content = BuildForm(
                ("grant_type", "password"),
                ("client_id", _settings.ClientId),
                ("client_secret", _settings.ClientSecret),
                ("username", username),
                ("password", password),
                ("scope", "openid email profile")
            );

            return await PostTokenAsync(content);
        }

        public async Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken)
        {
            var content = BuildForm(
                ("grant_type", "refresh_token"),
                ("client_id", _settings.ClientId),
                ("client_secret", _settings.ClientSecret),
                ("refresh_token", refreshToken)
            );

            return await PostTokenAsync(content);
        }

        public async Task<Result> RevokeTokenAsync(string refreshToken)
        {
            var content = BuildForm(
                ("client_id", _settings.ClientId),
                ("client_secret", _settings.ClientSecret),
                ("refresh_token", refreshToken)
            );

            var req = new HttpRequestMessage(HttpMethod.Post, GetEndpoint("logout"))
            {
                Content = content
            };

            return await SendKeycloakRequestAsync(req, AuthErrors.Session.EndFailed);
        }

        public async Task<Result<UserInfoDto>> GetUserInfoAsync(string accessToken)
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

        private string GetEndpoint(string path) =>
            $"{_settings.AuthServerUrl}/realms/{_settings.Realm}/protocol/openid-connect/{path}";

        private static FormUrlEncodedContent BuildForm(params (string key, string value)[] pairs) =>
            new(pairs.ToDictionary(p => p.key, p => p.value));

        private async Task<Result<TokenResponse>> PostTokenAsync(FormUrlEncodedContent content)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, GetEndpoint("token")) { Content = content };
            return await SendKeycloakRequestAsync<TokenResponse>(req, AuthErrors.Token.InvalidCredentials);
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
