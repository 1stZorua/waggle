using Grpc.Core;
using Waggle.Common.Grpc;
using Waggle.Common.Results.Core;
using Waggle.Contracts.Favorite.Grpc;
using Waggle.Contracts.Favorite.Interfaces;

namespace Waggle.Contracts.Favorite.Clients
{
    public class FavoriteDataClient : IFavoriteDataClient
    {
        private readonly GrpcFavorite.GrpcFavoriteClient _client;

        public FavoriteDataClient(GrpcFavorite.GrpcFavoriteClient client)
        {
            _client = client;
        }

        public async Task<Result<GetFavoritesResponse>> GetFavoritesAsync(GetFavoritesRequest request)
        {
            try
            {
                var response = await _client.GetFavoritesAsync(request);
                return Result<GetFavoritesResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetFavoritesResponse>(ex);
            }
        }

        public async Task<Result<GetFavoriteByIdResponse>> GetFavoriteByIdAsync(GetFavoriteByIdRequest request)
        {
            try
            {
                var response = await _client.GetFavoriteByIdAsync(request);
                return Result<GetFavoriteByIdResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetFavoriteByIdResponse>(ex);
            }
        }

        public async Task<Result<GetFavoritesByUserIdResponse>> GetFavoritesByUserIdAsync(GetFavoritesByUserIdRequest request)
        {
            try
            {
                var response = await _client.GetFavoritesByUserIdAsync(request);
                return Result<GetFavoritesByUserIdResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetFavoritesByUserIdResponse>(ex);
            }
        }

        public async Task<Result<GetFavoritesByTargetResponse>> GetFavoritesByTargetAsync(GetFavoritesByTargetRequest request)
        {
            try
            {
                var response = await _client.GetFavoritesByTargetAsync(request);
                return Result<GetFavoritesByTargetResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetFavoritesByTargetResponse>(ex);
            }
        }

        public async Task<Result<GetFavoriteCountsResponse>> GetFavoriteCountsAsync(GetFavoriteCountsRequest request)
        {
            try
            {
                var response = await _client.GetFavoriteCountsAsync(request);
                return Result<GetFavoriteCountsResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetFavoriteCountsResponse>(ex);
            }
        }

        public async Task<Result<HasFavoritedResponse>> HasFavoritedAsync(HasFavoritedRequest request)
        {
            try
            {
                var response = await _client.HasFavoritedAsync(request);
                return Result<HasFavoritedResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<HasFavoritedResponse>(ex);
            }
        }

        public async Task<Result<CreateFavoriteResponse>> CreateFavoriteAsync(CreateFavoriteRequest request)
        {
            try
            {
                var response = await _client.CreateFavoriteAsync(request);
                return Result<CreateFavoriteResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<CreateFavoriteResponse>(ex);
            }
        }

        public async Task<Result> DeleteFavoriteAsync(DeleteFavoriteRequest request)
        {
            try
            {
                await _client.DeleteFavoriteAsync(request);
                return Result.Ok();
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException(ex);
            }
        }
    }
}