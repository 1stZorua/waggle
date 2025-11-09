using Waggle.AuthService.Dtos;

namespace Waggle.AuthService.Tests.TestUtils
{
    public static class DummyUser
    {
        public static RegisterRequestDto Create()
        {
            return new()
            {
                Username = "testuser",
                Email = "test@gmail.com",
                FirstName = "Test",
                LastName = "User",
                Password = "password"
            };
        }
    }
}
