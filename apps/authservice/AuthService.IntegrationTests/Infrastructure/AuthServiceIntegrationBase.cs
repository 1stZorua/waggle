using MassTransit.Testing;
using Moq;
using Sprache;
using System.Net;
using System.Net.Http.Headers;
using Waggle.AuthService.Constants;
using Waggle.AuthService.Dtos;
using Waggle.Common.Models;
using Waggle.Contracts.User.Extensions;
using Waggle.Testing.Infrastructure.Base;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit.Abstractions;

namespace Waggle.AuthService.IntegrationTests.Infrastructure
{
    public abstract class AuthServiceIntegrationBase : HttpIntegrationTestBase, IClassFixture<CustomWebAppFactory>
    {
        protected readonly CustomWebAppFactory Factory;
        protected readonly ITestOutputHelper Output;
        protected ITestHarness TestHarness => Factory.Services.GetTestHarness();

        protected AuthServiceIntegrationBase(CustomWebAppFactory factory, ITestOutputHelper output)
            : base(factory.Services, factory.CreateClient())
        {
            Factory = factory;
            Output = output;
        }

        protected override string GetEndpoint(string action) => $"/api/auth/{action}";

        public override async Task InitializeAsync()
        {
            Factory.WireMockServer.Reset();
            await base.InitializeAsync();
        }

        #region Test Data Helpers

        protected static string UniqueUsername(string prefix = "testuser")
            => $"{prefix}_{Guid.NewGuid():N}";

        protected static RegisterRequestDto CreateRegisterRequest(
            string? username = null,
            string? email = null,
            string? firstName = null,
            string? lastName = null,
            string? password = null) => new()
            {
                Username = username ?? UniqueUsername(),
                Email = email ?? TestConstants.MockEmail,
                FirstName = firstName ?? "Test",
                LastName = lastName ?? "User",
                Password = password ?? "Password123!"
            };

        #endregion

        #region WireMock Setup Methods

        private void SetupAdminToken()
        {
            Factory.WireMockServer
                .Given(Request.Create()
                    .WithPath("/realms/test-realm/protocol/openid-connect/token")
                    .WithBody("*grant_type=client_credentials*")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{
                        ""access_token"":""admin-token"",
                        ""expires_in"":3600,
                        ""token_type"":""Bearer""
                    }"));
        }

        protected void SetupSuccessfulUserRegistration(Guid? userId = null)
        {
            var actualUserId = userId ?? Guid.NewGuid();

            SetupAdminToken();

            Factory.WireMockServer
                .Given(Request.Create()
                    .WithPath("/admin/realms/test-realm/users")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(201)
                    .WithHeader("Location", $"{Factory.WireMockServer.Urls.First()}/admin/realms/test-realm/users/{actualUserId}"));
        }

        protected void SetupDuplicateUserError(string username)
        {
            SetupAdminToken();

            Factory.WireMockServer
                .Given(Request.Create()
                    .WithPath("/admin/realms/test-realm/users")
                    .WithBody($"*\"username\":\"{username}\"*")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(409)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{
                        ""errorMessage"":""User exists with same username""
                    }"));
        }

        protected void SetupSuccessfulLogin()
        {
            Factory.WireMockServer
                .Given(Request.Create()
                    .WithPath("/realms/test-realm/protocol/openid-connect/token")
                    .WithBody("*grant_type=password*")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{
                        ""access_token"":""access-token-123"",
                        ""refresh_token"":""refresh-token-123"",
                        ""expires_in"":3600,
                        ""refresh_expires_in"":7200,
                        ""token_type"":""Bearer""
                    }"));
        }

        protected void SetupFailedLogin()
        {
            Factory.WireMockServer
                .Given(Request.Create()
                    .WithPath("/realms/test-realm/protocol/openid-connect/token")
                    .WithBody("*grant_type=password*")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(401)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{
                        ""error"":""invalid_grant"",
                        ""error_description"":""Invalid user credentials""
                    }"));
        }

        protected void SetupSuccessfulRefresh()
        {
            Factory.WireMockServer
                .Given(Request.Create()
                    .WithPath("/realms/test-realm/protocol/openid-connect/token")
                    .WithBody("*grant_type=refresh_token*")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{
                        ""access_token"":""new-access-token"",
                        ""refresh_token"":""new-refresh-token"",
                        ""expires_in"":3600,
                        ""refresh_expires_in"":7200,
                        ""token_type"":""Bearer""
                    }"));
        }

        protected void SetupFailedRefresh()
        {
            Factory.WireMockServer
                .Given(Request.Create()
                    .WithPath("/realms/test-realm/protocol/openid-connect/token")
                    .WithBody("*grant_type=refresh_token*")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(400)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{
                        ""error"":""invalid_grant"",
                        ""error_description"":""Invalid refresh token""
                    }"));
        }

        protected void SetupSuccessfulLogout()
        {
            Factory.WireMockServer
                .Given(Request.Create()
                    .WithPath("/realms/test-realm/protocol/openid-connect/logout")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(204));
        }

        protected void SetupFailedLogout()
        {
            Factory.WireMockServer
                .Given(Request.Create()
                    .WithPath("/realms/test-realm/protocol/openid-connect/logout")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(400)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{
                        ""error"":""invalid_request""
                    }"));
        }

        protected void SetupSuccessfulTokenValidation()
        {
            Factory.WireMockServer
                .Given(Request.Create()
                    .WithPath("/realms/test-realm/protocol/openid-connect/userinfo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody($@"{{
                        ""sub"":""{TestConstants.MockUserId}"",
                        ""preferred_username"":""{TestConstants.MockUsername}"",
                        ""email"":""{TestConstants.MockEmail}"",
                        ""name"":""{TestConstants.MockName}"",
                        ""realm_access"":{{""roles"":[""user"",""admin""]}}
                    }}"));
        }

        protected void SetupFailedTokenValidation()
        {
            Factory.WireMockServer
                .Given(Request.Create()
                    .WithPath("/realms/test-realm/protocol/openid-connect/userinfo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(401)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{
                        ""error"":""invalid_token""
                    }"));
        }

        protected void SetupSuccessfulUserDeletion(Guid userId)
        {
            SetupAdminToken();

            Factory.WireMockServer
                .Given(Request.Create()
                    .WithPath($"/admin/realms/test-realm/users/{userId}")
                    .UsingDelete())
                .RespondWith(Response.Create()
                    .WithStatusCode(204));
        }

        protected void SetupFailedUserDeletion(Guid userId)
        {
            SetupAdminToken();

            Factory.WireMockServer
                .Given(Request.Create()
                    .WithPath($"/admin/realms/test-realm/users/{userId}")
                    .UsingDelete())
                .RespondWith(Response.Create()
                    .WithStatusCode(500)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ 
                        ""error"": ""Internal server error"" 
                    }"));
        }

        #endregion

        #region gRPC Mock Setup Methods

        protected void SetupUserDataDeletion(Guid userId, bool success = true)
        {
            var result = success
                ? Common.Results.Core.Result.Ok()
                : Common.Results.Core.Result.Fail(AuthErrors.User.DeletionFailed);

            Factory.UserDataClientMock
                .Setup(x => x.DeleteUserAsync(new() { Id = userId.ToString() }))
                .Returns(Task.FromResult(result));
        }

        protected void VerifyUserDataDeletionCalled(Guid userId, Times? times = null)
        {
            Factory.UserDataClientMock
                .Verify(x => x.DeleteUserAsync(new() { Id = userId.ToString() }), times ?? Times.Once());
        }

        #endregion

        #region Register Helper Methods

        protected Task<ApiResponse<RegisterResponseDto>> RegisterUserAsync(RegisterRequestDto request)
            => PostAsync<RegisterResponseDto, RegisterRequestDto>("register", request);

        protected Task<ApiResponse<RegisterResponseDto>> RegisterUserExpectingFailureAsync(RegisterRequestDto request)
            => PostAsync<RegisterResponseDto, RegisterRequestDto>("register", request, expectSuccess: false);

        #endregion

        #region Login Helper Methods

        protected Task<ApiResponse<TokenResponseDto>> LoginAsync(LoginRequestDto request)
            => PostAsync<TokenResponseDto, LoginRequestDto>("login", request);

        protected Task<ApiResponse<TokenResponseDto>> LoginExpectingFailureAsync(LoginRequestDto request)
            => PostAsync<TokenResponseDto, LoginRequestDto>("login", request, expectSuccess: false);

        #endregion

        #region Refresh Helper Methods

        protected Task<ApiResponse<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
            => PostAsync<TokenResponseDto, RefreshTokenRequestDto>("refresh", request);

        protected Task<ApiResponse<TokenResponseDto>> RefreshTokenExpectingFailureAsync(RefreshTokenRequestDto request)
            => PostAsync<TokenResponseDto, RefreshTokenRequestDto>("refresh", request, expectSuccess: false);

        #endregion

        #region Logout Helper Methods

        protected Task<ApiResponse> LogoutAsync(LogoutRequestDto request)
            => PostAsync("logout", request);

        protected Task<ApiResponse> LogoutExpectingFailureAsync(LogoutRequestDto request)
            => PostAsync("logout", request, expectSuccess: false);

        #endregion

        #region Validate Helper Methods

        protected Task<ApiResponse> ValidateTokenAsync(string bearerToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetEndpoint("validate"));
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(bearerToken);
            return SendRequestAsync(request);
        }

        protected Task<ApiResponse> ValidateTokenExpectingFailureAsync(string bearerToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetEndpoint("validate"));
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(bearerToken);
            return SendRequestAsync(request, expectSuccess: false);
        }

        protected async Task<HttpStatusCode> ValidateTokenForGatewayAsync(string bearerToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetEndpoint("validate"));
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(bearerToken);
            request.Headers.Add("X-ForwardAuth", "true");

            var response = await Client.SendAsync(request);

            return response.StatusCode;
        }

        protected async Task<Dictionary<string, string>> ValidateTokenAndGetHeadersAsync(string bearerToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetEndpoint("validate"));
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(bearerToken);

            var response = await Client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var headers = new Dictionary<string, string>();
            foreach (var header in response.Headers)
            {
                if (header.Key.StartsWith("X-User-"))
                    headers[header.Key] = string.Join(",", header.Value);
            }

            return headers;
        }

        #endregion

        #region Delete User Helper Methods

        protected Task<ApiResponse> DeleteUserAsync(Guid userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, GetEndpoint($"{userId}"));
            return SendRequestAsync(request);
        }

        protected Task<ApiResponse> DeleteUserExpectingFailureAsync(Guid userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, GetEndpoint($"{userId}"));
            return SendRequestAsync(request, expectSuccess: false);
        }

        #endregion
    }
}