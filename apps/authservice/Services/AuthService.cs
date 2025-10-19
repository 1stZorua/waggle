using AuthService.Dtos;
using AuthService.Models;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using UserService.Grpc;
using Waggle.Common.Constants;
using Waggle.Common.Results;
using Waggle.Contracts.User.Interfaces;

namespace AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _http;
        private readonly KeycloakSettings _settings;
        private readonly IUserDataClient _userClient;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AuthService(HttpClient http, IOptions<KeycloakSettings> opts, IMapper mapper, IUserDataClient userClient)
        {
            _http = http;
            _settings = opts.Value;
            _mapper = mapper;
            _userClient = userClient;
        }

        public async Task<Result<RegisterResponseDto>> CreateUserAsync(RegisterRequestDto request)
        {
            var tokenResult = await PostTokenAsync(BuildForm(
                ("grant_type", "client_credentials"),
                ("client_id", _settings.AdminClientId),
                ("client_secret", _settings.AdminClientSecret)
            ));

            if (!tokenResult.Success)
                return Result<RegisterResponseDto>.Fail(tokenResult.Message ?? "Failed to get admin token");

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
                "Failed to create user",
                location => location.Split('/').Last()
            );

            if (!keycloakResult.Success)
                return Result<RegisterResponseDto>.Fail(keycloakResult.Message ?? "Failed to create user", keycloakResult.ErrorCode);

            var keycloakUserId = Guid.Parse(keycloakResult.Data!);

            // 3. Create user in UserService
            var userCreateRequest = new CreateUserRequest
            {
                Id = keycloakUserId.ToString(),
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var userServiceResult = await _userClient.CreateUserAsync(userCreateRequest);

            if (!userServiceResult.Success)
            {
                // Compensating transaction: Delete user from Keycloak
                await DeleteKeycloakUserAsync(token.AccessToken, keycloakUserId.ToString());
                return Result<RegisterResponseDto>.Fail(
                    userServiceResult.Message ?? "Failed to create user record",
                    userServiceResult.ErrorCode);
            }

            // 4. Return success with user ID
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
            return await SendKeycloakRequestAsync(req, "Failed to logout");
        }

        public async Task<Result<UserInfoDto>> ValidateAsync(ValidateTokenRequestDto request)
        {
            var bearerToken = request.BearerToken;
            if (string.IsNullOrWhiteSpace(bearerToken) || !bearerToken.StartsWith("Bearer "))
                return Result<UserInfoDto>.Fail("Missing or invalid token", ErrorCodes.InvalidInput);

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
            var result = await SendKeycloakRequestAsync<TokenResponse>(req, "Failed to parse token response");

            if (!result.Success)
                return Result<TokenResponseDto>.Fail(result.Message ?? "Failed to get token", result.ErrorCode);

            var dto = _mapper.Map<TokenResponseDto>(result.Data!);
            return Result<TokenResponseDto>.Ok(dto);
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

        private async Task<Result> DeleteKeycloakUserAsync(string adminToken, string userId)
        {
            var req = new HttpRequestMessage(
                HttpMethod.Delete,
                $"{_settings.AuthServerUrl}/admin/realms/{_settings.Realm}/users/{userId}")
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", adminToken) }
            };

            return await SendKeycloakRequestAsync(req, "Failed to delete user from Keycloak");
        }


        private async Task<Result<T>> SendKeycloakRequestAsync<T>(HttpRequestMessage request, string failureMessage)
        {
            try
            {
                var res = await _http.SendAsync(request);
                var json = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                    return Result<T>.FromHttpStatus(res.StatusCode, failureMessage);

                try
                {
                    var data = JsonSerializer.Deserialize<T?>(json, _jsonOptions)
                        ?? throw new JsonException("Response body is null");

                    return Result<T>.Ok(data);
                }
                catch (JsonException)
                {
                    return Result<T>.Fail(failureMessage, ErrorCodes.ServiceFailed);
                }
            }
            catch (HttpRequestException)
            {
                return Result<T>.Fail("Cannot connect to Keycloak", ErrorCodes.ServiceUnavailable);
            }
            catch (Exception)
            {
                return Result<T>.Fail(failureMessage, ErrorCodes.ServiceFailed);
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
            catch (HttpRequestException)
            {
                return Result.Fail("Cannot connect to Keycloak", ErrorCodes.ServiceUnavailable);
            }
            catch
            {
                return Result.Fail(failureMessage, ErrorCodes.ServiceFailed);
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
                    return Result<T>.Fail("Resource created but location not returned");

                var result = mapLocationToResult(location);
                return Result<T>.Ok(result);
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
