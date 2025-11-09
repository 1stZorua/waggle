using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Waggle.AuthService.IntegrationTests.Infrastructure;
using Waggle.Common.Models;
using Waggle.Common.Pagination.Models;
using Waggle.Testing.Infrastructure.Base;
using Waggle.UserService.Data;
using Waggle.UserService.Dtos;
using Waggle.UserService.Models;

namespace Waggle.UserService.IntegrationTests.Infrastructure
{
    public abstract class UserServiceIntegrationTestBase
        : EfCoreIntegrationTestBase<UserDbContext>
    {
        protected readonly CustomWebAppFactory Factory;

        protected UserServiceIntegrationTestBase(CustomWebAppFactory factory)
            : base(factory.Services, factory.CreateClient())
        {
            Factory = factory;
        }

        #region Database Helpers

        protected override async Task CleanDatabaseCoreAsync(UserDbContext context)
        {
            context.Users.RemoveRange(context.Users);
            await context.SaveChangesAsync();
        }

        protected Task<User> SeedUserAsync(User user) => SeedEntityAsync(user);

        protected Task<List<User>> SeedUsersAsync(IEnumerable<User> users) => SeedEntitiesAsync(users);

        protected Task<List<User>> SeedUsersAsync(int count = TestConstants.SmallDataSet)
        {
            var users = Enumerable.Range(1, count).Select(i => new User
            {
                Id = Guid.NewGuid(),
                Username = $"testuser{i}",
                Email = $"test{i}@{TestConstants.DefaultEmailDomain}",
                FirstName = $"{TestConstants.DefaultFirstName}{i}",
                LastName = $"{TestConstants.DefaultLastName}{i}",
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                UpdatedAt = DateTime.UtcNow.AddDays(-i)
            }).ToList();

            return SeedEntitiesAsync(users);
        }

        protected async Task<User?> GetUserFromDatabaseAsync(Guid id)
        {
            using var scope = Factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            return await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        protected async Task<List<User>> GetAllUsersFromDatabaseAsync()
        {
            using var scope = Factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            return await context.Users.AsNoTracking().ToListAsync();
        }

        #endregion

        #region User API Helpers

        protected Task<ApiResponse<PagedResult<UserDto>>> GetAllUsersAsync(PaginationRequest request)
            => GetPagedAsync<UserDto>("users", request);

        protected Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid id)
            => SendRequestAsync<UserDto>(new(HttpMethod.Get, GetEndpoint($"users/{id}")));

        protected Task<ApiResponse<UserDto>> GetUserByIdExpectingFailureAsync(Guid id)
            => SendRequestAsync<UserDto>(new(HttpMethod.Get, GetEndpoint($"users/{id}")), expectSuccess: false);

        protected Task<ApiResponse<UserDto>> CreateUserAsync(UserCreateDto dto)
            => PostAsync<UserDto, UserCreateDto>("users", dto);

        protected Task<ApiResponse<UserDto>> CreateUserExpectingFailureAsync(UserCreateDto dto)
            => PostAsync<UserDto, UserCreateDto>("users", dto, expectSuccess: false);

        #endregion

        #region Test Data Builders

        protected static UserCreateDto CreateValidUserDto(Guid? id = null, string? username = null, string? email = null)
        {
            var guid = id ?? Guid.NewGuid();
            return new UserCreateDto
            {
                Id = guid,
                Username = username ?? $"user_{guid:N}"[..20],
                Email = email ?? $"user_{guid:N}@{TestConstants.DefaultEmailDomain}",
                FirstName = TestConstants.DefaultFirstName,
                LastName = TestConstants.DefaultLastName
            };
        }

        protected static User CreateUser(Guid? id = null, string? username = null, string? email = null)
        {
            var guid = id ?? Guid.NewGuid();
            return new User
            {
                Id = guid,
                Username = username ?? $"user_{guid:N}"[..20],
                Email = email ?? $"user_{guid:N}@{TestConstants.DefaultEmailDomain}",
                FirstName = TestConstants.DefaultFirstName,
                LastName = TestConstants.DefaultLastName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        #endregion
    }
}