using FluentAssertions;
using MassTransit.Testing;
using System.Net;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.IntegrationTests.Infrastructure;
using Waggle.Common.Constants;
using Waggle.Common.Models;
using Waggle.Contracts.Auth.Events;
using Xunit.Abstractions;

namespace Waggle.AuthService.IntegrationTests
{
    public class AuthControllerTests : AuthServiceIntegrationBase
    {
        public AuthControllerTests(CustomWebAppFactory factory, ITestOutputHelper output)
            : base(factory, output) { }

        #region Register Tests

        [Fact]
        public async Task Register_ShouldReturnCreatedUser_AndPublishEvent()
        {
            // Arrange
            var request = CreateRegisterRequest(
                firstName: "John",
                lastName: "Doe"
            );
            SetupSuccessfulUserRegistration();

            // Act
            var result = await RegisterUserAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Success);
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().NotBeNullOrEmpty();
            result.Data.Username.Should().Be(request.Username);
            result.Data.Email.Should().Be(request.Email);
            result.Data.FirstName.Should().Be(request.FirstName);
            result.Data.LastName.Should().Be(request.LastName);

            var publishedEvent = TestHarness.Published
                .Select<RegisteredEvent>()
                .Where(x => x.Context.Message.Username == request.Username)
                .FirstOrDefault()?.Context.Message;

            publishedEvent.Should().NotBeNull();
            publishedEvent.Id.ToString().Should().Be(result.Data.Id);
            publishedEvent.Email.Should().Be(request.Email);
            publishedEvent.Username.Should().Be(request.Username);
            publishedEvent.FirstName.Should().Be(request.FirstName);
            publishedEvent.LastName.Should().Be(request.LastName);
        }

        [Fact]
        public async Task Register_DuplicateUsername_ShouldReturnFailureResult()
        {
            // Arrange
            var username = UniqueUsername("duplicate");
            var request = CreateRegisterRequest(
                username: username,
                email: "duplicate@example.com",
                firstName: "John",
                lastName: "Doe"
            );

            SetupSuccessfulUserRegistration();
            var firstResult = await RegisterUserAsync(request);
            firstResult.Status.Should().Be(ApiStatus.Success);

            await InitializeAsync();
            SetupDuplicateUserError(username);

            // Act
            var result = await RegisterUserExpectingFailureAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Fail);
            result.Code.Should().Be(ErrorCodes.AlreadyExists);
        }

        #endregion

        #region Login Tests

        [Fact]
        public async Task Login_ValidCredentials_ShouldReturnTokens()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new LoginRequestDto
            {
                Identifier = "testuser",
                Password = "Password123!"
            };

            SetupSuccessfulLogin(userId);
            SetupUserDataClientGetUser(userId, success: true);

            // Act
            var result = await LoginAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Success);
            result.Data.Should().NotBeNull();
            result.Data!.AccessToken.Should().NotBeNullOrEmpty();
            result.Data.RefreshToken.Should().NotBeNullOrEmpty();

            VerifyUserDataClientGetUserCalled(userId);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Identifier = "testuser",
                Password = "WrongPassword"
            };

            SetupFailedLogin();

            // Act
            var result = await LoginExpectingFailureAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Fail);
            result.Code.Should().Be(ErrorCodes.Unauthorized);
        }

        [Fact]
        public async Task Login_WhenUserNotFoundInUserService_ShouldReturnNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new LoginRequestDto
            {
                Identifier = "testuser",
                Password = "Password123!"
            };

            SetupSuccessfulLogin(userId);
            SetupUserDataClientGetUser(userId, success: false);

            // Act
            var result = await LoginExpectingFailureAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Fail);
            result.Code.Should().Be(ErrorCodes.NotFound);

            VerifyUserDataClientGetUserCalled(userId);
        }

        #endregion

        #region Refresh Token Tests

        [Fact]
        public async Task Refresh_ValidRefreshToken_ShouldReturnNewTokens()
        {
            // Arrange
            var request = new RefreshTokenRequestDto
            {
                RefreshToken = TestConstants.ValidRefreshToken
            };

            SetupSuccessfulRefresh();

            // Act
            var result = await RefreshTokenAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Success);
            result.Data.Should().NotBeNull();
            result.Data!.AccessToken.Should().NotBeNullOrEmpty();
            result.Data.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Refresh_InvalidRefreshToken_ShouldReturnUnauthorized()
        {
            // Arrange
            var request = new RefreshTokenRequestDto
            {
                RefreshToken = "invalid-refresh-token"
            };

            SetupFailedRefresh();

            // Act
            var result = await RefreshTokenExpectingFailureAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Fail);
            result.Code.Should().Be(ErrorCodes.InvalidInput);
        }

        #endregion

        #region Logout Tests

        [Fact]
        public async Task Logout_ValidRefreshToken_ShouldSucceed()
        {
            // Arrange
            var request = new LogoutRequestDto
            {
                RefreshToken = TestConstants.ValidRefreshToken
            };

            SetupSuccessfulLogout();

            // Act
            var result = await LogoutAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Success);
        }

        [Fact]
        public async Task Logout_InvalidRefreshToken_ShouldReturnError()
        {
            // Arrange
            var request = new LogoutRequestDto
            {
                RefreshToken = "invalid-refresh-token"
            };

            SetupFailedLogout();

            // Act
            var result = await LogoutExpectingFailureAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Fail);
        }

        #endregion

        #region Validate Tests

        [Fact]
        public async Task Validate_ValidToken_ShouldReturnSuccess()
        {
            // Arrange
            SetupSuccessfulTokenValidation();

            // Act
            var result = await ValidateTokenAsync(TestConstants.ValidBearerToken);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Success);
        }

        [Fact]
        public async Task Validate_InvalidToken_ShouldReturnUnauthorized()
        {
            // Arrange
            var token = "Bearer invalid-token";

            Factory.WireMockServer
                .Given(WireMock.RequestBuilders.Request.Create()
                    .WithPath("/realms/test-realm/protocol/openid-connect/userinfo")
                    .UsingGet())
                .RespondWith(WireMock.ResponseBuilders.Response.Create()
                    .WithStatusCode(401)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{""error"":""invalid_token""}"));

            // Act
            var result = await ValidateTokenExpectingFailureAsync(token);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Fail);
            result.Code.Should().Be(ErrorCodes.Unauthorized);
        }

        [Fact]
        public async Task Validate_WithGatewayHeader_ShouldReturnOkStatus()
        {
            // Arrange
            SetupSuccessfulTokenValidation();

            // Act
            var statusCode = await ValidateTokenForGatewayAsync(TestConstants.ValidBearerToken);

            // Assert
            statusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Validate_ValidToken_ShouldSetUserHeaders()
        {
            // Arrange
            SetupSuccessfulTokenValidation();

            // Act
            var headers = await ValidateTokenAndGetHeadersAsync(TestConstants.ValidBearerToken);

            // Assert
            headers.Should().ContainKey("X-User-ID");
            headers.Should().ContainKey("X-User-Username");
            headers.Should().ContainKey("X-User-Email");
            headers.Should().ContainKey("X-User-Name");

            if (headers.TryGetValue("X-User-Roles", out string? value))
                value.Should().NotBeNull();
        }

        #endregion

        #region Delete User Tests

        [Fact]
        public async Task DeleteUser_ShouldSucceed_AndPublishEvent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupSuccessfulUserDeletion(userId);

            // Act
            var result = await DeleteUserAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Success);

            var publishedEvent = TestHarness.Published
                .Select<DeletedEvent>()
                .Where(x => x.Context.Message.Id == userId)
                .FirstOrDefault()?.Context.Message;

            publishedEvent.Should().NotBeNull();
            publishedEvent.Id.Should().Be(userId);
        }

        [Fact]
        public async Task DeleteUser_WhenKeycloakFails_ShouldReturnError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupFailedUserDeletion(userId);

            // Act
            var result = await DeleteUserExpectingFailureAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Error);
            result.Code.Should().Be(ErrorCodes.ServiceFailed);

            var publishedEvent = TestHarness.Published
                .Select<DeletedEvent>()
                .Where(x => x.Context.Message.Id == userId)
                .FirstOrDefault();

            publishedEvent.Should().BeNull();
        }

        #endregion
    }
}