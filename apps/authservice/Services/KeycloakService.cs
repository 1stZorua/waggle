using System.Net.Http.Headers;
using System.Text.Json;
using AuthService.Dtos;
using AuthService.Models;
using Microsoft.Extensions.Options;
using Waggle.Common.Constants;
using Waggle.Common.Models;

namespace AuthService.Services
{
    public class KeycloakService : IKeycloakService
    {
        private readonly HttpClient _http;
        private readonly KeycloakSettings _settings;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public KeycloakService(HttpClient http, IOptions<KeycloakSettings> opts)
        {
            _http = http;
            _settings = opts.Value;
        }

        public async Task<Result> CreateUserAsync(string username, string email, string password)
        {
            var tokenResult = await PostTokenAsync(BuildForm(
                ("grant_type", "client_credentials"),
                ("client_id", _settings.AdminClientId),
                ("client_secret", _settings.AdminClientSecret)
            ));

            if (!tokenResult.Success)
                return Result.Fail(tokenResult.Message ?? "Failed to get admin token");

            var token = tokenResult.Data!;

            var user = new
            {
                username,
                email,
                enabled = true,
                credentials = new[] { new { type = "password", value = password, temporary = false } }
            };

            var json = JsonSerializer.Serialize(user);
            var requestContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var req = new HttpRequestMessage(HttpMethod.Post, $"{_settings.AuthServerUrl}/admin/realms/{_settings.Realm}/users")
            {
                Content = requestContent,
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken) }
            };

            return await SendKeycloakRequestAsync(req, "Failed to create user");
        }

        public async Task<Result<TokenResponse>> PasswordGrantAsync(string username, string password)
        {
            var content = BuildForm(
                ("grant_type", "password"),
                ("client_id", _settings.ClientId),
                ("client_secret", _settings.ClientSecret),
                ("username", username),
                ("password", password)
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

        public async Task<Result> LogoutAsync(string refreshToken)
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
            return await SendKeycloakRequestAsync(
                request: req, 
                failureMessage: "Failed to logout", 
                successMessage: "Successfully logged out"
            );
        }

        public async Task<Result<UserInfoDto>> ValidateAsync(string? bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken) || !bearerToken.StartsWith("Bearer "))
                return Result<UserInfoDto>.Fail("Missing or invalid token", ErrorCodes.InvalidInput);

            var token = bearerToken["Bearer ".Length..].Trim();
            return await GetUserInfoAsync(token);
        }

        private string GetEndpoint(string path) =>
            $"{_settings.AuthServerUrl}/realms/{_settings.Realm}/protocol/openid-connect/{path}";

        private static FormUrlEncodedContent BuildForm(params (string key, string value)[] pairs) =>
            new(pairs.ToDictionary(p => p.key, p => p.value));

        private async Task<Result<TokenResponse>> PostTokenAsync(FormUrlEncodedContent content)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, GetEndpoint("token")) { Content = content };
            return await SendKeycloakRequestAsync<TokenResponse>(req, "Failed to parse token response");
        }

        private async Task<Result<UserInfoDto>> GetUserInfoAsync(string? accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                return Result<UserInfoDto>.Fail("Access token is null", ErrorCodes.InvalidInput);

            var req = new HttpRequestMessage(HttpMethod.Get, GetEndpoint("userinfo"))
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", accessToken) }
            };

            return await SendKeycloakRequestAsync<UserInfoDto>(req, "Failed to parse user info");
        }

        private async Task<Result> SendKeycloakRequestAsync(HttpRequestMessage request, string failureMessage, string? successMessage = null)
        {
            try
            {
                var res = await _http.SendAsync(request);

                if (!res.IsSuccessStatusCode)
                    return Result.FromHttpStatus(res.StatusCode, failureMessage);

                return Result.Ok(successMessage);
            }
            catch (HttpRequestException)
            {
                return Result.Fail("Cannot connect to Keycloak", ErrorCodes.ServiceUnavailable);
            }
            catch
            {
                return Result.Fail(failureMessage, ErrorCodes.ServiceFailed);
            }
        }

        private async Task<Result<T>> SendKeycloakRequestAsync<T>(HttpRequestMessage request, string failureMessage, string? successMessage = null)
        {
            try
            {
                var res = await _http.SendAsync(request);
                var json = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                    return Result<T>.FromHttpStatus(res.StatusCode, failureMessage);

                var data = JsonSerializer.Deserialize<T?>(json, _jsonOptions)
                    ?? throw new JsonException("Response body is null");

                return Result<T>.Ok(successMessage, data);
            }
            catch (HttpRequestException)
            {
                return Result<T>.Fail("Cannot connect to Keycloak", ErrorCodes.ServiceUnavailable);
            }
            catch
            {
                return Result<T>.Fail(failureMessage, ErrorCodes.ServiceFailed);
            }
        }
    }
}
