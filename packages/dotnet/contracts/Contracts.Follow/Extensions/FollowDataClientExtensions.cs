using Waggle.Common.Results.Core;
using Waggle.Contracts.Follow.Grpc;
using Waggle.Contracts.Follow.Interfaces;

namespace Waggle.Contracts.Follow.Extensions
{
    public static class FollowDataClientExtensions
    {
        public static async Task<Result<GetFollowByIdResponse>> GetFollowByIdAsync(
            this IFollowDataClient client,
            Guid id)
        {
            return await client.GetFollowByIdAsync(new GetFollowByIdRequest { Id = id.ToString() });
        }

        public static async Task<Result> DeleteFollowAsync(
            this IFollowDataClient client,
            Guid id)
        {
            return await client.DeleteFollowAsync(new DeleteFollowRequest { Id = id.ToString() });
        }
    }
}
