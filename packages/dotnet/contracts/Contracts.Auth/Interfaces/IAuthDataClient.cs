using Google.Protobuf.WellKnownTypes;
using Waggle.Common.Results.Core;
using Waggle.Contracts.Auth.Grpc;

namespace Waggle.Contracts.Auth.Interfaces
{
    public interface IAuthDataClient
    {
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
        Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request);
        Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<Result<Empty>> LogoutAsync(LogoutRequest request);
        Task<Result<ValidateTokenResponse>> ValidateAsync(ValidateTokenRequest request);
    }
}