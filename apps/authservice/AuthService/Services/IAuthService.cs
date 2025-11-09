using Waggle.AuthService.Dtos;
using Waggle.Common.Results.Core;

namespace Waggle.AuthService.Services
{
    public interface IAuthService
    {
        Task<Result<RegisterResponseDto>> CreateUserAsync(RegisterRequestDto request);
        Task<Result<TokenResponseDto>> PasswordGrantAsync(LoginRequestDto request);
        Task<Result<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<Result> LogoutAsync(LogoutRequestDto request);
        Task<Result<UserInfoDto>> ValidateAsync(ValidateTokenRequestDto request);
    }
}
