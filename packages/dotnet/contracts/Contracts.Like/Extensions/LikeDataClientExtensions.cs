using Waggle.Common.Results.Core;
using Waggle.Contracts.Like.Interfaces;
using Waggle.Contracts.Like.Grpc;

namespace Waggle.Contracts.Like.Extensions
{
    public static class LikeDataClientExtensions
    {
        public static async Task<Result<GetLikeByIdResponse>> GetLikeByIdAsync(
            this ILikeDataClient client,
            Guid id)
        {
            return await client.GetLikeByIdAsync(new GetLikeByIdRequest { Id = id.ToString() });
        }

        public static async Task<Result> DeleteLikeAsync(
            this ILikeDataClient client,
            Guid id)
        {
            return await client.DeleteLikeAsync(new DeleteLikeRequest { Id = id.ToString() });
        }

        public static async Task<Result<GetLikeCountsResponse>> GetLikeCountsAsync(
            this ILikeDataClient client,
            IEnumerable<Guid> targetIds)
        {
            var request = new GetLikeCountsRequest();
            request.TargetIds.AddRange(targetIds.Select(id => id.ToString()));
            return await client.GetLikeCountsAsync(request);
        }
    }
}
