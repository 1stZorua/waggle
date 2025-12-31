using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Waggle.Common.Grpc;
using Waggle.Common.Results.Core;
using Waggle.Contracts.Auth.Grpc;
using Waggle.Contracts.Auth.Interfaces;

namespace Waggle.Contracts.Auth.Clients
{
    public class AuthDataClient : IAuthDataClient
    {
        private readonly GrpcAuth.GrpcAuthClient _client;

        public AuthDataClient(GrpcAuth.GrpcAuthClient client)
        {
            _client = client;
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _client.LoginAsync(request);
                return Result<LoginResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<LoginResponse>(ex);
            }
        }

        public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _client.RegisterAsync(request);
                return Result<RegisterResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<RegisterResponse>(ex);
            }
        }

        public async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                var response = await _client.RefreshTokenAsync(request);
                return Result<RefreshTokenResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<RefreshTokenResponse>(ex);
            }
        }

        public async Task<Result<Empty>> LogoutAsync(LogoutRequest request)
        {
            try
            {
                var response = await _client.LogoutAsync(request);
                return Result<Empty>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<Empty>(ex);
            }
        }

        public async Task<Result<ValidateTokenResponse>> ValidateAsync(ValidateTokenRequest request)
        {
            try
            {
                var response = await _client.ValidateAsync(request);
                return Result<ValidateTokenResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<ValidateTokenResponse>(ex);
            }
        }

        public async Task<Result<Empty>> DeleteUserAsync(DeleteUserRequest request)
        {
            try
            {
                var response = await _client.DeleteUserAsync(request);
                return Result<Empty>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<Empty>(ex);
            }
        }
    }
}