using Grpc.Core;
using Waggle.Common.Grpc;
using Waggle.Common.Results.Core;
using Waggle.Contracts.Like.Grpc;
using Waggle.Contracts.Like.Interfaces;

namespace Waggle.PostService.Grpc
{
    public class LikeDataClient : ILikeDataClient
    {
        private readonly GrpcLike.GrpcLikeClient _client;

        public LikeDataClient(GrpcLike.GrpcLikeClient client)
        {
            _client = client;
        }

        public async Task<Result<GetLikesResponse>> GetLikesAsync(GetLikesRequest request)
        {
            try
            {
                var response = await _client.GetLikesAsync(request);
                return Result<GetLikesResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetLikesResponse>(ex);
            }
        }

        public async Task<Result<GetLikeByIdResponse>> GetLikeByIdAsync(GetLikeByIdRequest request)
        {
            try
            {
                var response = await _client.GetLikeByIdAsync(request);
                return Result<GetLikeByIdResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetLikeByIdResponse>(ex);
            }
        }

        public async Task<Result<GetLikesByTargetIdResponse>> GetLikesByTargetIdAsync(GetLikesByTargetIdRequest request)
        {
            try
            {
                var response = await _client.GetLikesByTargetIdAsync(request);
                return Result<GetLikesByTargetIdResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetLikesByTargetIdResponse>(ex);
            }
        }

        public async Task<Result<GetLikesByUserIdResponse>> GetLikesByUserIdAsync(GetLikesByUserIdRequest request)
        {
            try
            {
                var response = await _client.GetLikesByUserIdAsync(request);
                return Result<GetLikesByUserIdResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetLikesByUserIdResponse>(ex);
            }
        }

        public async Task<Result<GetLikeCountResponse>> GetLikeCountAsync(GetLikeCountRequest request)
        {
            try
            {
                var response = await _client.GetLikeCountAsync(request);
                return Result<GetLikeCountResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetLikeCountResponse>(ex);
            }
        }

        public async Task<Result<HasLikedResponse>> HasLikedAsync(HasLikedRequest request)
        {
            try
            {
                var response = await _client.HasLikedAsync(request);
                return Result<HasLikedResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<HasLikedResponse>(ex);
            }
        }

        public async Task<Result<CreateLikeResponse>> CreateLikeAsync(CreateLikeRequest request)
        {
            try
            {
                var response = await _client.CreateLikeAsync(request);
                return Result<CreateLikeResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<CreateLikeResponse>(ex);
            }
        }

        public async Task<Result> DeleteLikeAsync(DeleteLikeRequest request)
        {
            try
            {
                await _client.DeleteLikeAsync(request);
                return Result.Ok();
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException(ex);
            }
        }
    }
}