using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Waggle.Common.Results;
using Waggle.Contracts.User.Interfaces;
using Waggle.Common.Grpc;
using Waggle.Contracts.User.Grpc;

namespace AuthService.SyncDataServices.Grpc
{
    public class UserDataClient : IUserDataClient
    {
        private readonly GrpcUser.GrpcUserClient _client;

        public UserDataClient(GrpcUser.GrpcUserClient client)
        {
            _client = client;
        }

        public async Task<Result<GetUserByIdResponse>> GetUserByIdAsync(GetUserByIdRequest request)
        {
            try
            {
                var response = await _client.GetUserByIdAsync(request);
                return Result<GetUserByIdResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetUserByIdResponse>(ex);
            }
        }

        public async Task<Result<CreateUserResponse>> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                var response = await _client.CreateUserAsync(request);
                return Result<CreateUserResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<CreateUserResponse>(ex);
            }
        }

        public async Task<Result<GetAllUsersResponse>> GetAllUsersAsync()
        {
            try
            {
                var response = await _client.GetAllUsersAsync(new Empty());
                return Result<GetAllUsersResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetAllUsersResponse>(ex);
            }
        }
    }
}