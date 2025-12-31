using Waggle.Common.Results.Core;
using Waggle.Contracts.Favorite.Grpc;

namespace Waggle.Contracts.Favorite.Interfaces
{
    public interface IFavoriteDataClient
    {
        Task<Result<GetFavoritesResponse>> GetFavoritesAsync(GetFavoritesRequest request);
        Task<Result<GetFavoriteByIdResponse>> GetFavoriteByIdAsync(GetFavoriteByIdRequest request);
        Task<Result<GetFavoritesByUserIdResponse>> GetFavoritesByUserIdAsync(GetFavoritesByUserIdRequest request);
        Task<Result<GetFavoritesByTargetResponse>> GetFavoritesByTargetAsync(GetFavoritesByTargetRequest request);
        Task<Result<GetFavoriteCountsResponse>> GetFavoriteCountsAsync(GetFavoriteCountsRequest request);
        Task<Result<HasFavoritedResponse>> HasFavoritedAsync(HasFavoritedRequest request);
        Task<Result<CreateFavoriteResponse>> CreateFavoriteAsync(CreateFavoriteRequest request);
        Task<Result> DeleteFavoriteAsync(DeleteFavoriteRequest request);
    }
}