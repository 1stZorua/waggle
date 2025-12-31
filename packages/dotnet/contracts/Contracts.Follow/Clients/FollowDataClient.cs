using Grpc.Core;
using Waggle.Common.Grpc;
using Waggle.Common.Results.Core;
using Waggle.Contracts.Follow.Grpc;
using Waggle.Contracts.Follow.Interfaces;

namespace Waggle.Contracts.Follow.Clients
{
    public class FollowDataClient : IFollowDataClient
    {
        private readonly GrpcFollow.GrpcFollowClient _client;

        public FollowDataClient(GrpcFollow.GrpcFollowClient client)
        {
            _client = client;
        }

        public async Task<Result<GetFollowsResponse>> GetFollowsAsync(GetFollowsRequest request)
        {
            try
            {
                var response = await _client.GetFollowsAsync(request);
                return Result<GetFollowsResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetFollowsResponse>(ex);
            }
        }

        public async Task<Result<GetFollowByIdResponse>> GetFollowByIdAsync(GetFollowByIdRequest request)
        {
            try
            {
                var response = await _client.GetFollowByIdAsync(request);
                return Result<GetFollowByIdResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetFollowByIdResponse>(ex);
            }
        }

        public async Task<Result<GetFollowersResponse>> GetFollowersAsync(GetFollowersRequest request)
        {
            try
            {
                var response = await _client.GetFollowersAsync(request);
                return Result<GetFollowersResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetFollowersResponse>(ex);
            }
        }

        public async Task<Result<GetFollowingResponse>> GetFollowingAsync(GetFollowingRequest request)
        {
            try
            {
                var response = await _client.GetFollowingAsync(request);
                return Result<GetFollowingResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetFollowingResponse>(ex);
            }
        }

        public async Task<Result<GetFollowerCountsResponse>> GetFollowerCountsAsync(GetFollowerCountsRequest request)
        {
            try
            {
                var response = await _client.GetFollowerCountsAsync(request);
                return Result<GetFollowerCountsResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetFollowerCountsResponse>(ex);
            }
        }

        public async Task<Result<GetFollowingCountsResponse>> GetFollowingCountsAsync(GetFollowingCountsRequest request)
        {
            try
            {
                var response = await _client.GetFollowingCountsAsync(request);
                return Result<GetFollowingCountsResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetFollowingCountsResponse>(ex);
            }
        }

        public async Task<Result<IsFollowingResponse>> IsFollowingAsync(IsFollowingRequest request)
        {
            try
            {
                var response = await _client.IsFollowingAsync(request);
                return Result<IsFollowingResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<IsFollowingResponse>(ex);
            }
        }

        public async Task<Result<CreateFollowResponse>> CreateFollowAsync(CreateFollowRequest request)
        {
            try
            {
                var response = await _client.CreateFollowAsync(request);
                return Result<CreateFollowResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<CreateFollowResponse>(ex);
            }
        }

        public async Task<Result> DeleteFollowAsync(DeleteFollowRequest request)
        {
            try
            {
                await _client.DeleteFollowAsync(request);
                return Result.Ok();
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException(ex);
            }
        }
    }
}