using Waggle.Common.Results.Core;
using Waggle.Contracts.Like.Grpc;

namespace Waggle.Contracts.Like.Interfaces
{
    public interface ILikeDataClient
    {
        Task<Result<GetLikesResponse>> GetLikesAsync(GetLikesRequest request);
        Task<Result<GetLikeByIdResponse>> GetLikeByIdAsync(GetLikeByIdRequest request);
        Task<Result<GetLikesByUserIdResponse>> GetLikesByUserIdAsync(GetLikesByUserIdRequest request);
        Task<Result<GetLikesByTargetIdResponse>> GetLikesByTargetIdAsync(GetLikesByTargetIdRequest request);
        Task<Result<GetLikeCountsResponse>> GetLikeCountsAsync(GetLikeCountsRequest request);
        Task<Result<HasLikedResponse>> HasLikedAsync(HasLikedRequest request);
        Task<Result<CreateLikeResponse>> CreateLikeAsync(CreateLikeRequest request);
        Task<Result> DeleteLikeAsync(DeleteLikeRequest request);
    }
}