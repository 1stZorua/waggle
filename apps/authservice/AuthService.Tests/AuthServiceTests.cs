using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Waggle.AuthService.Constants;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.Models;
using Waggle.AuthService.Services;
using Waggle.AuthService.Tests.TestUtils;
using Waggle.Common.Constants;
using Waggle.Common.Results.Core;
using Waggle.Contracts.Auth.Events;
using Waggle.Contracts.User.Grpc;
using Waggle.Contracts.User.Interfaces;

namespace Waggle.AuthService.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IKeycloakClient> _keycloakClientMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<IUserDataClient> _userDataClientMock;
        private readonly Mock<ILogger<Services.AuthService>> _loggerMock;
        private readonly IAuthService _service;

        public AuthServiceTests()
        {
            _keycloakClientMock = new Mock<IKeycloakClient>();
            _mapperMock = new Mock<IMapper>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _userDataClientMock = new Mock<IUserDataClient>();
            _loggerMock = new Mock<ILogger<Services.AuthService>>();

            _service = new Services.AuthService(
                _keycloakClientMock.Object,
                _mapperMock.Object,
                _publishEndpointMock.Object,
                _userDataClientMock.Object,
                _loggerMock.Object
            );

            SetupDefaultMapperMocks();
        }

        private void SetupDefaultMapperMocks()
        {
            _mapperMock.Setup(m => m.Map<RegisteredEvent>(It.IsAny<RegisterRequestDto>()))
                .Returns(new RegisteredEvent());

            _mapperMock.Setup(m => m.Map<TokenResponseDto>(It.IsAny<TokenResponse>()))
                .Returns<TokenResponse>(t => new TokenResponseDto
                {
                    AccessToken = t.AccessToken,
                    RefreshToken = t.RefreshToken,
                    ExpiresIn = t.ExpiresIn,
                    TokenType = t.TokenType
                });
        }

        #region CreateUserAsync Tests

        [Fact]
        public async Task CreateUserAsync_WhenSuccessful_ReturnsSuccessWithUserId()
        {
            // Arrange
            var request = DummyUser.Create();
            var adminToken = "admin-token";
            var userId = Guid.NewGuid();

            _mapperMock.Setup(m => m.Map<RegisterResponseDto>(It.IsAny<RegisterRequestDto>()))
                .Returns<RegisterRequestDto>(r => new RegisterResponseDto
                {
                    Id = userId.ToString(),
                    Username = r.Username,
                    Email = r.Email,
                    FirstName = r.FirstName,
                    LastName = r.LastName
                });

            _mapperMock.Setup(m => m.Map<RegisteredEvent>(It.IsAny<RegisterRequestDto>()))
                .Returns(new RegisteredEvent { Id = userId });

            _keycloakClientMock.Setup(c => c.GetAdminTokenAsync())
                .ReturnsAsync(Result<string>.Ok(adminToken));

            _keycloakClientMock.Setup(c => c.CreateUserAsync(It.IsAny<KeycloakUserRequest>(), It.IsAny<string>()))
                .ReturnsAsync(Result<Guid>.Ok(userId));

            // Act
            var result = await _service.CreateUserAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(userId.ToString());
            result.Data.Username.Should().Be(request.Username);
            result.Data.Email.Should().Be(request.Email);
            result.Data.FirstName.Should().Be(request.FirstName);
            result.Data.LastName.Should().Be(request.LastName);

            _keycloakClientMock.Verify(c => c.CreateUserAsync(
                It.IsAny<KeycloakUserRequest>(),
                It.IsAny<string>()), Times.Once);

            _publishEndpointMock.Verify(p => p.Publish(
                It.Is<RegisteredEvent>(e => e.Id == userId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_WhenAdminTokenFails_ReturnsFailure()
        {
            // Arrange
            var request = DummyUser.Create();

            _keycloakClientMock.Setup(c => c.GetAdminTokenAsync())
                .ReturnsAsync(Result<string>.Fail("Token failed", ErrorCodes.Unauthorized));

            // Act
            var result = await _service.CreateUserAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCodes.Unauthorized);

            _keycloakClientMock.Verify(c => c.CreateUserAsync(It.IsAny<KeycloakUserRequest>(), It.IsAny<string>()), Times.Never);
            _publishEndpointMock.Verify(p => p.Publish(It.IsAny<RegisteredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateUserAsync_WhenUserCreationFails_ReturnsFailure()
        {
            // Arrange
            var request = DummyUser.Create();

            var adminToken = "admin-token";

            _keycloakClientMock.Setup(c => c.GetAdminTokenAsync())
                .ReturnsAsync(Result<string>.Ok(adminToken));

            _keycloakClientMock.Setup(c => c.CreateUserAsync(It.IsAny<KeycloakUserRequest>(), adminToken))
                .ReturnsAsync(Result<Guid>.Fail(AuthErrors.User.AlreadyExists, ErrorCodes.AlreadyExists));

            // Act
            var result = await _service.CreateUserAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCodes.AlreadyExists);

            _publishEndpointMock.Verify(p => p.Publish(It.IsAny<RegisteredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateUserAsync_WhenEventPublishFails_DeletesUserAndReturnsFailure()
        {
            // Arrange
            var request = DummyUser.Create();

            var adminToken = "admin-token";
            var userId = Guid.NewGuid();

            _keycloakClientMock.Setup(c => c.GetAdminTokenAsync())
                .ReturnsAsync(Result<string>.Ok(adminToken));

            _keycloakClientMock.Setup(c => c.CreateUserAsync(It.IsAny<KeycloakUserRequest>(), adminToken))
                .ReturnsAsync(Result<Guid>.Ok(userId));

            _publishEndpointMock.Setup(p => p.Publish(It.IsAny<RegisteredEvent>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("MassTransit error"));

            _keycloakClientMock.Setup(c => c.DeleteUserAsync(userId, adminToken))
                .ReturnsAsync(Result.Ok());

            // Act
            var result = await _service.CreateUserAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);

            _keycloakClientMock.Verify(c => c.DeleteUserAsync(userId, adminToken), Times.Once);
        }

        #endregion

        #region PasswordGrantAsync Tests

        [Fact]
        public async Task PasswordGrantAsync_WhenSuccessful_ReturnsTokens()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Username = "user",
                Password = "password123"
            };

            var tokenResponse = new TokenResponse
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token",
                ExpiresIn = 3600,
                TokenType = "Bearer"
            };

            _keycloakClientMock.Setup(c => c.GetPasswordTokenAsync(request.Username, request.Password))
                .ReturnsAsync(Result<TokenResponse>.Ok(tokenResponse));

            // Act
            var result = await _service.PasswordGrantAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Data!.AccessToken.Should().Be(tokenResponse.AccessToken);
            result.Data.RefreshToken.Should().Be(tokenResponse.RefreshToken);
        }

        [Fact]
        public async Task PasswordGrantAsync_WhenInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Username = "user",
                Password = "wrong"
            };

            _keycloakClientMock.Setup(c => c.GetPasswordTokenAsync(request.Username, request.Password))
                .ReturnsAsync(Result<TokenResponse>.Fail(ErrorCodes.InvalidInput, ErrorCodes.Unauthorized));

            // Act
            var result = await _service.PasswordGrantAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCodes.Unauthorized);
        }

        #endregion

        #region RefreshTokenAsync Tests

        [Fact]
        public async Task RefreshTokenAsync_WhenSuccessful_ReturnsTokens()
        {
            // Arrange
            var request = new RefreshTokenRequestDto { RefreshToken = "token" };
            var tokenResponse = new TokenResponse
            {
                AccessToken = "new-access",
                RefreshToken = "new-refresh",
                ExpiresIn = 3600,
                TokenType = "Bearer"
            };

            _keycloakClientMock.Setup(c => c.RefreshTokenAsync(request.RefreshToken))
                .ReturnsAsync(Result<TokenResponse>.Ok(tokenResponse));

            // Act
            var result = await _service.RefreshTokenAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Data!.AccessToken.Should().Be(tokenResponse.AccessToken);
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenTokenExpired_ReturnsUnauthorized()
        {
            // Arrange
            var request = new RefreshTokenRequestDto { RefreshToken = "expired" };

            _keycloakClientMock.Setup(c => c.RefreshTokenAsync(request.RefreshToken))
                .ReturnsAsync(Result<TokenResponse>.Fail("Expired", ErrorCodes.Unauthorized));

            // Act
            var result = await _service.RefreshTokenAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCodes.Unauthorized);
        }

        #endregion

        #region LogoutAsync Tests

        [Fact]
        public async Task LogoutAsync_WhenSuccessful_ReturnsSuccess()
        {
            // Arrange
            var request = new LogoutRequestDto { RefreshToken = "token" };

            _keycloakClientMock.Setup(c => c.RevokeTokenAsync(request.RefreshToken))
                .ReturnsAsync(Result.Ok());

            // Act
            var result = await _service.LogoutAsync(request);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task LogoutAsync_WhenFails_ReturnsFailure()
        {
            // Arrange
            var request = new LogoutRequestDto { RefreshToken = "bad-token" };

            _keycloakClientMock.Setup(c => c.RevokeTokenAsync(request.RefreshToken))
                .ReturnsAsync(Result.Fail("Failed", ErrorCodes.ServiceFailed));

            // Act
            var result = await _service.LogoutAsync(request);

            // Assert
            result.Success.Should().BeFalse();
        }

        #endregion

        #region ValidateAsync Tests

        [Fact]
        public async Task ValidateAsync_WhenValidToken_ReturnsUserInfo()
        {
            // Arrange
            var request = new ValidateTokenRequestDto { BearerToken = "Bearer valid" };
            var userInfo = new UserInfoDto
            {
                Sub = "id",
                Username = "test",
                Email = "test@gmail.com",
                Name = "test",
                Roles = ["admin"]
            };

            _keycloakClientMock.Setup(c => c.GetUserInfoAsync("valid"))
                .ReturnsAsync(Result<UserInfoDto>.Ok(userInfo));

            // Act
            var result = await _service.ValidateAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Data!.Username.Should().Be(userInfo.Username);
        }

        [Fact]
        public async Task ValidateAsync_WhenInvalidFormat_ReturnsFailure()
        {
            // Arrange
            var request = new ValidateTokenRequestDto { BearerToken = "invalid" };

            // Act
            var result = await _service.ValidateAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCodes.InvalidInput);
        }

        [Fact]
        public async Task ValidateAsync_WhenEmptyToken_ReturnsFailure()
        {
            // Arrange
            var request = new ValidateTokenRequestDto { BearerToken = "" };

            // Act
            var result = await _service.ValidateAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCodes.InvalidInput);
        }

        #endregion

        #region DeleteUserAsync Tests

        [Fact]
        public async Task DeleteUserAsync_WhenSuccessful_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var adminToken = "admin-token";

            _keycloakClientMock.Setup(c => c.GetAdminTokenAsync())
                .ReturnsAsync(Result<string>.Ok(adminToken));

            _keycloakClientMock.Setup(c => c.DeleteUserAsync(userId, adminToken))
                .ReturnsAsync(Result.Ok());

            _userDataClientMock
                .Setup(c => c.DeleteUserAsync(It.IsAny<DeleteUserRequest>()))
                .ReturnsAsync(Result.Ok());

            // Act
            var result = await _service.DeleteUserAsync(userId);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteUserAsync_WhenAdminTokenFails_ReturnsFailureWithAuthError()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _keycloakClientMock.Setup(c => c.GetAdminTokenAsync())
                .ReturnsAsync(Result<string>.Fail(AuthErrors.Token.AdminAccessFailed, ErrorCodes.Unauthorized));

            // Act
            var result = await _service.DeleteUserAsync(userId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(AuthErrors.Token.AdminAccessFailed);
            result.ErrorCode.Should().Be(ErrorCodes.Unauthorized);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenKeycloakDeletionFails_ReturnsFailureWithAuthError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var adminToken = "admin-token";

            _keycloakClientMock.Setup(c => c.GetAdminTokenAsync())
                .ReturnsAsync(Result<string>.Ok(adminToken));

            _keycloakClientMock.Setup(c => c.DeleteUserAsync(userId, adminToken))
                .ReturnsAsync(Result.Fail(AuthErrors.User.DeletionFailed, ErrorCodes.ServiceFailed));

            // Act
            var result = await _service.DeleteUserAsync(userId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(AuthErrors.User.DeletionFailed);
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenUserDataClientDeletionFails_ReturnsFailureWithServiceError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var adminToken = "admin-token";

            _keycloakClientMock.Setup(c => c.GetAdminTokenAsync())
                .ReturnsAsync(Result<string>.Ok(adminToken));

            _keycloakClientMock.Setup(c => c.DeleteUserAsync(userId, adminToken))
                .ReturnsAsync(Result.Ok());

            _userDataClientMock
                .Setup(c => c.DeleteUserAsync(It.Is<DeleteUserRequest>(r => r.Id == userId.ToString())))
                .ReturnsAsync(Result.Fail(AuthErrors.User.DeletionFailed, ErrorCodes.ServiceFailed));

            // Act
            var result = await _service.DeleteUserAsync(userId);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);
        }

        #endregion
    }
}
