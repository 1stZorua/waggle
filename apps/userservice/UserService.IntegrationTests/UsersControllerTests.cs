using FluentAssertions;
using Waggle.AuthService.IntegrationTests.Infrastructure;
using Waggle.Common.Models;
using Waggle.Common.Pagination.Models;
using Waggle.UserService.IntegrationTests.Infrastructure;

namespace Waggle.UserService.IntegrationTests
{
    public class UsersControllerTests : UserServiceIntegrationTestBase, IClassFixture<CustomWebAppFactory>
    {
        public UsersControllerTests(CustomWebAppFactory factory)
            : base(factory) { }

        #region GetAllUsers Tests

        [Fact]
        public async Task GetAllUsers_WithNoUsers_ShouldReturnEmptyList()
        {
            var result = await GetAllUsersAsync(new());

            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Success);
            result.Data!.Items.Should().BeEmpty();
            result.Data.PageInfo.HasNextPage.Should().BeFalse();
            result.Data.PageInfo.HasPreviousPage.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllUsers_WithSeededUsers_ShouldReturnUsers()
        {
            var seededUsers = await SeedUsersAsync(TestConstants.SmallDataSet);

            var result = await GetAllUsersAsync(new());

            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Success);
            result.Data!.Items.Should().HaveCount(TestConstants.SmallDataSet);
            result.Data.PageInfo.HasNextPage.Should().BeFalse();
            result.Data.PageInfo.HasPreviousPage.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllUsers_WithPagination_ShouldReturnMultiplePages()
        {
            const int totalUsers = TestConstants.MediumDataSet;
            const int pageSize = TestConstants.SmallPageSize;
            await SeedUsersAsync(totalUsers);

            var page1 = await GetAllUsersAsync(new() { PageSize = pageSize });
            var page2 = await GetAllUsersAsync(new()
            {
                PageSize = pageSize,
                Cursor = page1.Data!.PageInfo.NextCursor
            });
            var page3 = await GetAllUsersAsync(new()
            {
                PageSize = pageSize,
                Cursor = page2.Data!.PageInfo.NextCursor
            });

            // Page 1
            page1.Data!.Items.Should().HaveCount(pageSize);
            page1.Data.PageInfo.HasNextPage.Should().BeTrue();
            page1.Data.PageInfo.HasPreviousPage.Should().BeFalse();

            // Page 2
            page2.Data!.Items.Should().HaveCount(pageSize);
            page2.Data.PageInfo.HasNextPage.Should().BeTrue();
            page2.Data.PageInfo.HasPreviousPage.Should().BeTrue();

            // Page 3
            page3.Data!.Items.Should().HaveCount(2);
            page3.Data.PageInfo.HasNextPage.Should().BeFalse();
            page3.Data.PageInfo.HasPreviousPage.Should().BeTrue();

            var allPageIds = page1.Data.Items
                .Concat(page2.Data.Items)
                .Concat(page3.Data.Items)
                .Select(u => u.Id)
                .ToList();
            allPageIds.Should().OnlyHaveUniqueItems();
            allPageIds.Should().HaveCount(totalUsers);
        }

        [Fact]
        public async Task GetAllUsers_BackwardPagination_ShouldReturnCorrectItems()
        {
            const int pageSize = 3;
            await SeedUsersAsync(7);

            var page1 = await GetAllUsersAsync(new() { PageSize = pageSize });
            var page2 = await GetAllUsersAsync(new()
            {
                PageSize = pageSize,
                Cursor = page1.Data!.PageInfo.NextCursor
            });

            var backwardPage = await GetAllUsersAsync(new()
            {
                PageSize = pageSize,
                Cursor = page2.Data!.PageInfo.PreviousCursor,
                Direction = PaginationDirection.Backward
            });

            backwardPage.Data!.Items.Should().HaveCount(pageSize);

            var page1Ids = page1.Data.Items.Select(u => u.Id).OrderBy(id => id).ToList();
            var backwardIds = backwardPage.Data.Items.Select(u => u.Id).OrderBy(id => id).ToList();
            backwardIds.Should().BeEquivalentTo(page1Ids);
        }

        #endregion

        #region GetUserById Tests

        [Fact]
        public async Task GetUserById_ExistingUser_ShouldReturnUser()
        {
            var user = await SeedUserAsync(CreateUser());

            var result = await GetUserByIdAsync(user.Id);

            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Success);
            result.Data!.Id.Should().Be(user.Id);
            result.Data.Username.Should().Be(user.Username);
        }

        [Fact]
        public async Task GetUserById_NonExistentUser_ShouldReturnFail()
        {
            var nonExistentId = Guid.NewGuid();

            var result = await GetUserByIdExpectingFailureAsync(nonExistentId);

            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Fail);
            result.Data.Should().BeNull();
        }

        #endregion

        #region CreateUser Tests

        [Fact]
        public async Task CreateUser_ValidData_ShouldReturnCreatedUser()
        {
            var dto = CreateValidUserDto();

            var result = await CreateUserAsync(dto);

            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Success);
            result.Data!.Id.Should().Be(dto.Id);
            result.Data.Username.Should().Be(dto.Username);
        }

        [Fact]
        public async Task CreateUser_DuplicateEmail_ShouldReturnFail()
        {
            var existingUser = await SeedUserAsync(CreateUser());
            var dto = CreateValidUserDto(email: existingUser.Email);

            var result = await CreateUserExpectingFailureAsync(dto);

            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Fail);
            result.Data.Should().BeNull();
            result.Message.Should().NotBeNullOrEmpty();
        }

        #endregion

        #region DeleteUser Tests

        [Fact]
        public async Task DeleteUser_ExistingUser_ShouldSucceed()
        {
            // Arrange
            var user = await SeedUserAsync(CreateUser());

            // Act
            var result = await DeleteUserAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Success);

            var deletedUser = await GetUserFromDatabaseAsync(user.Id);
            deletedUser.Should().BeNull();
        }

        [Fact]
        public async Task DeleteUser_NonExistentUser_ShouldReturnFail()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await DeleteUserExpectingFailureAsync(nonExistentId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(ApiStatus.Fail);
        }

        [Fact]
        public async Task DeleteUser_WithSeededUsers_ShouldReduceCount()
        {
            // Arrange
            var users = await SeedUsersAsync(3);
            var userToDelete = users[1];

            // Act
            var deleteResult = await DeleteUserAsync(userToDelete.Id);
            var allUsersAfterDelete = await GetAllUsersAsync(new PaginationRequest { PageSize = 10 });

            // Assert
            deleteResult.Status.Should().Be(ApiStatus.Success);
            allUsersAfterDelete.Data.Should().NotBeNull();
            allUsersAfterDelete.Data.Items.Should().HaveCount(2);
            allUsersAfterDelete.Data.Items.Should().NotContain(u => u.Id == userToDelete.Id);
        }

        #endregion
    }
}