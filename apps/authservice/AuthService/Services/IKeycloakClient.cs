using Waggle.AuthService.Dtos;
using Waggle.AuthService.Models;
using Waggle.Common.Results.Core;

namespace Waggle.AuthService.Services
{
    public interface IKeycloakClient
    {
        Task<Result<string>> GetAdminTokenAsync();
        Task<Result<Guid>> CreateUserAsync(KeycloakUserRequest user, string adminToken);
        Task<Result> DeleteUserAsync(Guid userId, string adminToken);
        Task<Result<TokenResponse>> GetPasswordTokenAsync(string username, string password);
        Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken);
        Task<Result> RevokeTokenAsync(string refreshToken);
        Task<Result<UserInfoDto>> GetUserInfoAsync(string accessToken);
    }
}
