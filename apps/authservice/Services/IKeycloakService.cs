using AuthService.Dtos;
using AuthService.Models;
using Waggle.Common.Models;

namespace AuthService.Services
{
    public interface IKeycloakService
    {
        Task<Result> CreateUserAsync(string username, string email, string password);
        Task<Result<TokenResponse>> PasswordGrantAsync(string username, string password);
        Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken);
        Task<Result> LogoutAsync(string refreshToken);
        Task<Result<UserInfoDto>> ValidateAsync(string? bearerToken);
    }
}
