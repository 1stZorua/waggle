using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Waggle.AuthService.Constants;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.Models;
using Waggle.AuthService.Saga.Context;
using Waggle.AuthService.Services;
using Waggle.AuthService.Tests.TestUtils;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;
using Waggle.Common.Validation;
using Waggle.Contracts.Auth.Events;
using Waggle.Contracts.User.Grpc;
using Waggle.Contracts.User.Interfaces;

namespace Waggle.AuthService.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IKeycloakClient> _keycloakClientMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserDataClient> _userDataClientMock;
        private readonly Mock<IServiceValidator> _validatorMock;
        private readonly Mock<ISagaCoordinator<RegistrationSagaContext, RegisterResponseDto>> _registrationSagaMock;
        private readonly Mock<ISagaCoordinator<DeletionSagaContext>> _deletionSagaMock;
        private readonly Mock<ILogger<Services.AuthService>> _loggerMock;
        private readonly IAuthService _service;

        public AuthServiceTests()
        {
            _keycloakClientMock = new Mock<IKeycloakClient>();
            _mapperMock = new Mock<IMapper>();
            _userDataClientMock = new Mock<IUserDataClient>();
            _validatorMock = new Mock<IServiceValidator>();
            _registrationSagaMock = new Mock<ISagaCoordinator<RegistrationSagaContext, RegisterResponseDto>>();
            _deletionSagaMock = new Mock<ISagaCoordinator<DeletionSagaContext>>();
            _loggerMock = new Mock<ILogger<Services.AuthService>>();

            _service = new Services.AuthService(
                _keycloakClientMock.Object,
                _mapperMock.Object,
                _userDataClientMock.Object,
                _validatorMock.Object,
                _registrationSagaMock.Object,
                _deletionSagaMock.Object,
                _loggerMock.Object
            );

            SetupDefaultMapperMocks();
            SetupDefaultValidatorMocks();
        }

        private void SetupDefaultMapperMocks()
        {
            _mapperMock.Setup(m => m.Map<RegisteredEvent>(It.IsAny<RegisterRequestDto>()))
                .Returns(new RegisteredEvent());

            _mapperMock.Setup(m => m.Map<DeletedEvent>(It.IsAny<Guid>()))
                .Returns<Guid>(id => new DeletedEvent { Id = id });

            _mapperMock.Setup(m => m.Map<TokenResponseDto>(It.IsAny<TokenResponse>()))
                .Returns<TokenResponse>(t => new TokenResponseDto
                {
                    AccessToken = t.AccessToken,
                    RefreshToken = t.RefreshToken,
                    ExpiresIn = t.ExpiresIn,
                    TokenType = t.TokenType
                });

            _mapperMock.Setup(m => m.Map<RegisterResponseDto>(It.IsAny<RegisterRequestDto>()))
                .Returns<RegisterRequestDto>(r => new RegisterResponseDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = r.Username,
                    Email = r.Email,
                    FirstName = r.FirstName,
                    LastName = r.LastName
                });

            _mapperMock.Setup(m => m.Map<DeletionSagaContext>(It.IsAny<DeleteUserRequestDto>()))
                .Returns<DeleteUserRequestDto>(dto => new DeletionSagaContext { Id = dto.Id });
        }

        private void SetupDefaultValidatorMocks()
        {
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<RegisterRequestDto>()))
                .ReturnsAsync(Result.Ok());

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<LoginRequestDto>()))
                .ReturnsAsync(Result.Ok());
        }

        #region CreateUserAsync Tests

        [Fact]
        public async Task CreateUserAsync_WhenValidationFails_ReturnsFailure()
        {
            // Arrange
            var request = DummyUser.Create();
            _validatorMock.Setup(v => v.ValidateAsync(request))
                .ReturnsAsync(Result.Fail("Validation failed", ErrorCodes.ValidationFailed));

            // Act
            var result = await _service.CreateUserAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCodes.ValidationFailed);
        }

        [Fact]
        public async Task CreateUserAsync_WhenSuccessful_ExecutesSagaAndReturnsResponse()
        {
            // Arrange
            var request = DummyUser.Create();
            var sagaResponse = new RegisterResponseDto
            {
                Id = Guid.NewGuid().ToString(),
                Username = "test-user",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };
            _registrationSagaMock.Setup(s => s.ExecuteAsync(It.IsAny<RegistrationSagaContext>()))
                .ReturnsAsync(Result<RegisterResponseDto>.Ok(sagaResponse));

            // Act
            var result = await _service.CreateUserAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(sagaResponse);
            _registrationSagaMock.Verify(s => s.ExecuteAsync(It.IsAny<RegistrationSagaContext>()), Times.Once);
        }

        #endregion

        #region PasswordGrantAsync Tests

        [Fact]
        public async Task PasswordGrantAsync_WhenSuccessful_ReturnsTokensAndUserData()
        {
            var request = new LoginRequestDto { Identifier = "user", Password = "password123" };
            var tokenResponse = new TokenResponse
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token",
                ExpiresIn = 3600,
                TokenType = "Bearer"
            };
            var userInfo = new UserInfoDto { Sub = Guid.NewGuid().ToString(), Username = "test-user" };
            var userId = Guid.Parse(userInfo.Sub);

            _keycloakClientMock.Setup(c => c.GetPasswordTokenAsync(request.Identifier, request.Password))
                .ReturnsAsync(Result<TokenResponse>.Ok(tokenResponse));
            _keycloakClientMock.Setup(c => c.GetUserInfoAsync(tokenResponse.AccessToken))
                .ReturnsAsync(Result<UserInfoDto>.Ok(userInfo));
            _userDataClientMock.Setup(c => c.GetUserByIdAsync(It.Is<GetUserByIdRequest>(r => r.Id == userId.ToString())))
                .ReturnsAsync(Result<GetUserByIdResponse>.Ok(new GetUserByIdResponse { User = new() { Id = userId.ToString(), Username = request.Identifier } }));

            var result = await _service.PasswordGrantAsync(request);

            result.Success.Should().BeTrue();
            result.Data!.AccessToken.Should().Be(tokenResponse.AccessToken);
            result.Data.RefreshToken.Should().Be(tokenResponse.RefreshToken);
        }

        [Fact]
        public async Task PasswordGrantAsync_WhenInvalidCredentials_ReturnsUnauthorized()
        {
            var request = new LoginRequestDto { Identifier = "user", Password = "wrong" };
            _keycloakClientMock.Setup(c => c.GetPasswordTokenAsync(request.Identifier, request.Password))
                .ReturnsAsync(Result<TokenResponse>.Fail(AuthErrors.Token.InvalidCredentials, ErrorCodes.Unauthorized));

            var result = await _service.PasswordGrantAsync(request);

            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCodes.Unauthorized);
            result.Message.Should().Be(AuthErrors.Token.InvalidCredentials);
        }

        #endregion

        #region RefreshTokenAsync Tests

        [Fact]
        public async Task RefreshTokenAsync_WhenSuccessful_ReturnsTokens()
        {
            var request = new RefreshTokenRequestDto { RefreshToken = "token" };
            var tokenResponse = new TokenResponse { AccessToken = "new-access", RefreshToken = "new-refresh", ExpiresIn = 3600, TokenType = "Bearer" };
            _keycloakClientMock.Setup(c => c.RefreshTokenAsync(request.RefreshToken))
                .ReturnsAsync(Result<TokenResponse>.Ok(tokenResponse));

            var result = await _service.RefreshTokenAsync(request);

            result.Success.Should().BeTrue();
            result.Data!.AccessToken.Should().Be(tokenResponse.AccessToken);
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenTokenExpired_ReturnsUnauthorized()
        {
            var request = new RefreshTokenRequestDto { RefreshToken = "expired" };
            _keycloakClientMock.Setup(c => c.RefreshTokenAsync(request.RefreshToken))
                .ReturnsAsync(Result<TokenResponse>.Fail(AuthErrors.Token.Expired, ErrorCodes.Unauthorized));

            var result = await _service.RefreshTokenAsync(request);

            result.Success.Should().BeFalse();
            result.Message.Should().Be(AuthErrors.Token.Expired);
            result.ErrorCode.Should().Be(ErrorCodes.Unauthorized);
        }

        #endregion

        #region LogoutAsync Tests

        [Fact]
        public async Task LogoutAsync_WhenSuccessful_ReturnsSuccess()
        {
            var request = new LogoutRequestDto { RefreshToken = "token" };
            _keycloakClientMock.Setup(c => c.RevokeTokenAsync(request.RefreshToken))
                .ReturnsAsync(Result.Ok());

            var result = await _service.LogoutAsync(request);

            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task LogoutAsync_WhenFails_ReturnsFailure()
        {
            var request = new LogoutRequestDto { RefreshToken = "bad-token" };
            _keycloakClientMock.Setup(c => c.RevokeTokenAsync(request.RefreshToken))
                .ReturnsAsync(Result.Fail(AuthErrors.Token.Invalid, ErrorCodes.ServiceFailed));

            var result = await _service.LogoutAsync(request);

            result.Success.Should().BeFalse();
            result.Message.Should().Be(AuthErrors.Token.Invalid);
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);
        }

        #endregion

        #region ValidateAsync Tests

        [Fact]
        public async Task ValidateAsync_WhenValidToken_ReturnsUserInfo()
        {
            var request = new ValidateTokenRequestDto { BearerToken = "Bearer valid" };
            var userInfo = new UserInfoDto { Sub = "id", Username = "test", Roles = ["admin"] };

            _keycloakClientMock.Setup(c => c.GetUserInfoAsync("valid")).ReturnsAsync(Result<UserInfoDto>.Ok(userInfo));

            var result = await _service.ValidateAsync(request);

            result.Success.Should().BeTrue();
            result.Data!.Username.Should().Be(userInfo.Username);
        }

        [Fact]
        public async Task ValidateAsync_WhenInvalidFormat_ReturnsFailure()
        {
            var request = new ValidateTokenRequestDto { BearerToken = "invalid" };

            var result = await _service.ValidateAsync(request);

            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCodes.InvalidInput);
        }

        [Fact]
        public async Task ValidateAsync_WhenEmptyToken_ReturnsFailure()
        {
            var request = new ValidateTokenRequestDto { BearerToken = "" };

            var result = await _service.ValidateAsync(request);

            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCodes.InvalidInput);
        }

        #endregion

        #region DeleteUserAsync Tests

        [Fact]
        public async Task DeleteUserAsync_WhenUserIsAdmin_CallsSagaAndReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            var currentUser = new UserInfoDto { Sub = Guid.NewGuid().ToString(), Username = "admin", Roles = ["admin"] };

            _deletionSagaMock.Setup(s => s.ExecuteAsync(It.IsAny<DeletionSagaContext>()))
                .ReturnsAsync(Result.Ok());

            var result = await _service.DeleteUserAsync(new DeleteUserRequestDto { Id = userId }, currentUser);

            result.Success.Should().BeTrue();
            _deletionSagaMock.Verify(s => s.ExecuteAsync(It.Is<DeletionSagaContext>(c => c.Id == userId)), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenDeletingSelf_CallsSagaAndReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            var currentUser = new UserInfoDto { Sub = userId.ToString(), Username = "self", Roles = [] };

            _deletionSagaMock.Setup(s => s.ExecuteAsync(It.IsAny<DeletionSagaContext>()))
                .ReturnsAsync(Result.Ok());

            var result = await _service.DeleteUserAsync(new DeleteUserRequestDto { Id = userId }, currentUser);

            result.Success.Should().BeTrue();
            _deletionSagaMock.Verify(s => s.ExecuteAsync(It.Is<DeletionSagaContext>(c => c.Id == userId)), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenNotSelfAndNotAdmin_ReturnsForbidden()
        {
            var userId = Guid.NewGuid();
            var currentUser = new UserInfoDto { Sub = Guid.NewGuid().ToString(), Username = "user", Roles = [] };

            var result = await _service.DeleteUserAsync(new DeleteUserRequestDto { Id = userId }, currentUser);

            result.Success.Should().BeFalse();
            result.Message.Should().Be(ErrorMessages.Authentication.PermissionRequired);
            result.ErrorCode.Should().Be(ErrorCodes.Forbidden);
            _deletionSagaMock.Verify(s => s.ExecuteAsync(It.IsAny<DeletionSagaContext>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenSagaFails_ReturnsFailure()
        {
            var userId = Guid.NewGuid();
            var currentUser = new UserInfoDto { Sub = Guid.NewGuid().ToString(), Username = "admin", Roles = ["admin"] };

            _deletionSagaMock.Setup(s => s.ExecuteAsync(It.IsAny<DeletionSagaContext>()))
                .ReturnsAsync(Result.Fail("Saga failed", ErrorCodes.ServiceFailed));

            var result = await _service.DeleteUserAsync(new DeleteUserRequestDto { Id = userId }, currentUser);

            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);
            _deletionSagaMock.Verify(s => s.ExecuteAsync(It.Is<DeletionSagaContext>(c => c.Id == userId)), Times.Once);
        }

        #endregion
    }
}
