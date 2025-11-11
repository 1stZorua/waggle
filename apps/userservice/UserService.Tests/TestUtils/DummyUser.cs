using Waggle.Contracts.Auth.Events;
using Waggle.UserService.Dtos;
using Waggle.UserService.Models;

namespace Waggle.UserService.Tests.TestUtils
{
    public class DummyUser
    {
        public static User CreateEntity(
            Guid? id = null,
            string? username = null,
            string? email = null,
            string? firstName = null,
            string? lastName = null,
            DateTime? createdAt = null,
            DateTime? updatedAt = null)
        {
            return new User
            {
                Id = id ?? Guid.NewGuid(),
                Username = username ?? "testuser",
                Email = email ?? "test@gmail.com",
                FirstName = firstName ?? "Test",
                LastName = lastName ?? "User",
                CreatedAt = createdAt ?? DateTime.UtcNow,
                UpdatedAt = updatedAt ?? DateTime.UtcNow
            };
        }

        public static UserCreateDto CreateRequest(
            Guid? id = null,
            string? username = null,
            string? email = null,
            string? firstName = null,
            string? lastName = null)
        {
            return new UserCreateDto
            {
                Id = id ?? Guid.NewGuid(),
                Username = username ?? "testuser",
                Email = email ?? "test@gmail.com",
                FirstName = firstName ?? "Test",
                LastName = lastName ?? "User"
            };
        }

        public static UserDto CreateResult(
            Guid? id = null,
            string? username = null,
            string? email = null,
            string? firstName = null,
            string? lastName = null,
            DateTime? createdAt = null)
        {
            return new UserDto
            {
                Id = id ?? Guid.NewGuid(),
                Username = username ?? "testuser",
                Email = email ?? "test@test.com",
                FirstName = firstName ?? "Test",
                LastName = lastName ?? "User",
                CreatedAt = createdAt ?? DateTime.UtcNow
            };
        }

        public static RegisteredEvent CreateRegisteredEvent(
            Guid? id = null,
            string? username = null,
            string? email = null,
            string? firstName = null,
            string? lastName = null)
        {
            return new()
            {
                Id = id ?? Guid.NewGuid(),
                Username = username ?? "testuser",
                Email = email ?? "testuser@test.com",
                FirstName = firstName ?? "Test",
                LastName = lastName ?? "User"
            };
        }
    }
}
