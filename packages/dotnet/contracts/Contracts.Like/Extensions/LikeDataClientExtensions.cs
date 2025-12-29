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

        public static async Task<Result<GetLikeCountResponse>> GetLikeCountAsync(
            this ILikeDataClient client,
            Guid targetId)
        {
            return await client.GetLikeCountAsync(new GetLikeCountRequest { TargetId = targetId.ToString() });
        }
    }
}
