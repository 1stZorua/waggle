using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Waggle.Common.Constants;
using Waggle.Common.Pagination.Models;
using Waggle.Contracts.Auth.Events;
using Waggle.UserService.Constants;
using Waggle.UserService.Data;
using Waggle.UserService.Dtos;
using Waggle.UserService.Models;
using Waggle.UserService.Services;
using Waggle.UserService.Tests.TestUtils;

namespace Waggle.UserService.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<Services.UserService>> _mockLogger;
        private readonly IUserService _service;

        public UserServiceTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<Services.UserService>>();
            _service = new Services.UserService(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object);

            SetupDefaultMapperMocks();
        }

        private void SetupDefaultMapperMocks()
        {
            _mockMapper.Setup(m => m.Map<UserCreateDto>(It.IsAny<RegisteredEvent>()))
                .Returns<RegisteredEvent>(e => DummyUser.CreateRequest(id: e.Id));

            _mockMapper.Setup(m => m.Map<User>(It.IsAny<UserCreateDto>()))
                .Returns<UserCreateDto>(dto => DummyUser.CreateEntity(id: dto.Id));

            _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
                .Returns<User>(user => DummyUser.CreateResult(id: user.Id));

            _mockMapper.Setup(m => m.Map<List<UserDto>>(It.IsAny<IEnumerable<User>>()))
                .Returns<IEnumerable<User>>(users => users.Select(u => DummyUser.CreateResult(id: u.Id)).ToList());
        }

        #region GetAllUsersAsync Tests

        [Fact]
        public async Task GetAllUsersAsync_ReturnsOkResultWithUsers_WhenUsersExist()
        {
            // Arrange
            var users = new List<User>
            {
                DummyUser.CreateEntity(username: "user1", email: "user1@gmail.com"),
                DummyUser.CreateEntity(username: "user2", email: "user2@gmail.com")
            };

            var pagedResult = new PagedResult<User>
            {
                Items = users,
                PageInfo = new()
            };

            var paginationRequest = new PaginationRequest { PageSize = 10 };

            _mockRepo.Setup(r => r.GetAllUsersAsync(It.IsAny<PaginationRequest>()))
                     .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllUsersAsync(paginationRequest);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Items.Should().HaveCount(2);
            result.Data.PageInfo.HasNextPage.Should().BeFalse();
            result.Data.PageInfo.HasPreviousPage.Should().BeFalse();
            _mockRepo.Verify(r => r.GetAllUsersAsync(It.IsAny<PaginationRequest>()), Times.Once);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var pagedResult = new PagedResult<User>
            {
                Items = [],
                PageInfo = new ()
            };

            var paginationRequest = new PaginationRequest { PageSize = 10 };

            _mockRepo.Setup(r => r.GetAllUsersAsync(It.IsAny<PaginationRequest>()))
                     .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllUsersAsync(paginationRequest);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Items.Should().BeEmpty();
            result.Data.PageInfo.HasNextPage.Should().BeFalse();
            result.Data.PageInfo.HasPreviousPage.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsFailResult_WhenExceptionOccurs()
        {
            // Arrange
            var paginationRequest = new PaginationRequest { PageSize = 10 };
            _mockRepo.Setup(r => r.GetAllUsersAsync(It.IsAny<PaginationRequest>()))
                     .ThrowsAsync(new Exception());

            // Act
            var result = await _service.GetAllUsersAsync(paginationRequest);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(UserErrors.Service.Failed);
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsPagedResultWithNextPage_WhenMoreUsersExist()
        {
            // Arrange
            var users = new List<User>
            {
                DummyUser.CreateEntity(username: "user1", email: "user1@gmail.com"),
                DummyUser.CreateEntity(username: "user2", email: "user2@gmail.com")
            };

            var pagedResult = new PagedResult<User>
            {
                Items = users,
                PageInfo = new() { NextCursor = "next-cursor", HasNextPage = true }
            };

            var paginationRequest = new PaginationRequest { PageSize = 2 };

            _mockRepo.Setup(r => r.GetAllUsersAsync(It.IsAny<PaginationRequest>()))
                     .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllUsersAsync(paginationRequest);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Items.Should().HaveCount(2);
            result.Data.PageInfo.HasNextPage.Should().BeTrue();
            result.Data.PageInfo.NextCursor.Should().NotBeNullOrEmpty();
        }

        #endregion

        #region GetUserByIdAsync Tests

        [Fact]
        public async Task GetUserByIdAsync_ReturnsOkResultWithUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = DummyUser.CreateEntity(id: userId);

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            result.Success.Should().BeTrue();
            result.Data!.Id.Should().Be(userId);
            result.Data.Username.Should().Be(user.Username);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsFailResult_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null!);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(UserErrors.User.NotFound);
            result.ErrorCode.Should().Be(ErrorCodes.NotFound);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsFailResult_WhenExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ThrowsAsync(new Exception());

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(UserErrors.Service.Failed);
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);
        }

        #endregion

        #region CreateUserAsync Tests

        [Fact]
        public async Task CreateUserAsync_ReturnsOkResult_WhenUserCreatedSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createDto = DummyUser.CreateRequest(id: userId);
            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null!);
            _mockRepo.Setup(r => r.AddUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateUserAsync(createDto);

            // Assert
            result.Success.Should().BeTrue();
            result.Data!.Id.Should().Be(userId);
            _mockRepo.Verify(r => r.AddUserAsync(It.Is<User>(u =>
                u.Id == createDto.Id &&
                u.Username == createDto.Username &&
                u.Email == createDto.Email &&
                u.FirstName == createDto.FirstName &&
                u.LastName == createDto.LastName &&
                u.CreatedAt != default && u.UpdatedAt != default
            )), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsFailResult_WhenUserAlreadyExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createDto = DummyUser.CreateRequest(id: userId);
            var existingUser = DummyUser.CreateEntity(id: userId);

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(existingUser);

            // Act
            var result = await _service.CreateUserAsync(createDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(UserErrors.User.AlreadyExists);
            result.ErrorCode.Should().Be(ErrorCodes.AlreadyExists);
            _mockRepo.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsFailResult_WhenDuplicateKeyErrorOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createDto = DummyUser.CreateRequest(id: userId);
            var user = DummyUser.CreateEntity(id: userId);
            var dbUpdateException = new DbUpdateException("Error", new Exception("duplicate key value violates unique constraint"));

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null!);
            _mockRepo.Setup(r => r.AddUserAsync(It.IsAny<User>())).ThrowsAsync(dbUpdateException);

            // Act
            var result = await _service.CreateUserAsync(createDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(UserErrors.User.AlreadyExists);
            result.ErrorCode.Should().Be(ErrorCodes.AlreadyExists);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsFailResult_WhenDbUpdateExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createDto = DummyUser.CreateRequest(id: userId);
            var user = DummyUser.CreateEntity(id: userId);

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null!);
            _mockRepo.Setup(r => r.AddUserAsync(It.IsAny<User>())).ThrowsAsync(new DbUpdateException());

            // Act
            var result = await _service.CreateUserAsync(createDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(UserErrors.User.CreationFailed);
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsFailResult_WhenGeneralExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createDto = DummyUser.CreateRequest(id: userId);

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ThrowsAsync(new Exception());

            // Act
            var result = await _service.CreateUserAsync(createDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(UserErrors.Service.Failed);
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);
        }

        #endregion

        #region DeleteUserAsync Tests

        [Fact]
        public async Task DeleteUserAsync_ReturnsOk_WhenUserDeletedSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = DummyUser.CreateEntity(id: userId);

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _mockRepo.Setup(r => r.DeleteUserAsync(user)).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteUserAsync(userId);

            // Assert
            result.Success.Should().BeTrue();
            _mockRepo.Verify(r => r.DeleteUserAsync(It.Is<User>(u => u.Id == userId)), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsFail_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null!);

            // Act
            var result = await _service.DeleteUserAsync(userId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(UserErrors.User.NotFound);
            result.ErrorCode.Should().Be(ErrorCodes.NotFound);
            _mockRepo.Verify(r => r.DeleteUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsFail_WhenDbUpdateExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = DummyUser.CreateEntity(id: userId);

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _mockRepo.Setup(r => r.DeleteUserAsync(user)).ThrowsAsync(new DbUpdateException());

            // Act
            var result = await _service.DeleteUserAsync(userId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(UserErrors.User.DeletionFailed);
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsFail_WhenGeneralExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ThrowsAsync(new Exception());

            // Act
            var result = await _service.DeleteUserAsync(userId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(UserErrors.Service.Failed);
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);
        }

        #endregion

        #region HandleUserRegisteredAsync Tests

        [Fact]
        public async Task HandleUserRegisteredAsync_ReturnsOkResult_WhenUserCreatedSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var registeredEvent = DummyUser.CreateRegisteredEvent(id: userId);

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null!);
            _mockRepo.Setup(r => r.AddUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.HandleUserRegisteredAsync(registeredEvent);

            // Assert
            result.Success.Should().BeTrue();
            result.Data!.Id.Should().Be(userId);
            _mockRepo.Verify(r => r.AddUserAsync(It.Is<User>(u => u.Id == registeredEvent.Id)), Times.Once);
        }

        [Fact]
        public async Task HandleUserRegisteredAsync_ReturnsOkResult_WhenUserAlreadyExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = DummyUser.CreateEntity(id: userId);
            var registeredEvent = DummyUser.CreateRegisteredEvent(id: userId);

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(existingUser);

            // Act
            var result = await _service.HandleUserRegisteredAsync(registeredEvent);

            // Assert
            result.Success.Should().BeTrue();
            result.Data!.Id.Should().Be(userId);
            _mockRepo.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task HandleUserRegisteredAsync_ReturnsFailResult_WhenDbUpdateExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var registeredEvent = DummyUser.CreateRegisteredEvent(id: userId);

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null!);
            _mockRepo.Setup(r => r.AddUserAsync(It.IsAny<User>())).ThrowsAsync(new DbUpdateException());

            // Act
            var result = await _service.HandleUserRegisteredAsync(registeredEvent);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(UserErrors.User.CreationFailed);
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);
        }

        [Fact]
        public async Task HandleUserRegisteredAsync_ReturnsFailResult_WhenGeneralExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var registeredEvent = DummyUser.CreateRegisteredEvent(id: userId);

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ThrowsAsync(new Exception());

            // Act
            var result = await _service.HandleUserRegisteredAsync(registeredEvent);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(UserErrors.Service.Failed);
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);
        }

        #endregion

        #region HandleUserDeletedAsync Tests

        [Fact]
        public async Task HandleUserDeletedAsync_ReturnsOk_WhenUserDeletedSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var deletedEvent = new DeletedEvent { Id = userId, CreatedAt = DateTime.UtcNow, EventType = "DeletedEvent" };
            var user = DummyUser.CreateEntity(id: userId);

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _mockRepo.Setup(r => r.DeleteUserAsync(user)).Returns(Task.CompletedTask);

            // Act
            var result = await _service.HandleUserDeletedAsync(deletedEvent);

            // Assert
            result.Success.Should().BeTrue();
            _mockRepo.Verify(r => r.DeleteUserAsync(It.Is<User>(u => u.Id == userId)), Times.Once);
        }

        [Fact]
        public async Task HandleUserDeletedAsync_ReturnsOk_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var deletedEvent = new DeletedEvent { Id = userId, CreatedAt = DateTime.UtcNow, EventType = "DeletedEvent" };

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null!);

            // Act
            var result = await _service.HandleUserDeletedAsync(deletedEvent);

            // Assert
            result.Success.Should().BeTrue(); // idempotent behavior
            _mockRepo.Verify(r => r.DeleteUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task HandleUserDeletedAsync_ReturnsFail_WhenDbUpdateExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var deletedEvent = new DeletedEvent { Id = userId, CreatedAt = DateTime.UtcNow, EventType = "DeletedEvent" };
            var user = DummyUser.CreateEntity(id: userId);

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _mockRepo.Setup(r => r.DeleteUserAsync(user)).ThrowsAsync(new DbUpdateException());

            // Act
            var result = await _service.HandleUserDeletedAsync(deletedEvent);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(UserErrors.User.DeletionFailed);
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);
        }

        [Fact]
        public async Task HandleUserDeletedAsync_ReturnsFail_WhenGeneralExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var deletedEvent = new DeletedEvent { Id = userId, CreatedAt = DateTime.UtcNow, EventType = "DeletedEvent" };

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ThrowsAsync(new Exception());

            // Act
            var result = await _service.HandleUserDeletedAsync(deletedEvent);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(UserErrors.Service.Failed);
            result.ErrorCode.Should().Be(ErrorCodes.ServiceFailed);
        }

        #endregion
    }
}
