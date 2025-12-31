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

        public static async Task<Result<GetFollowerCountsResponse>> GetFollowerCountsAsync(
            this IFollowDataClient client,
            IEnumerable<Guid> userIds)
        {
            var request = new GetFollowerCountsRequest();
            request.UserIds.AddRange(userIds.Select(id => id.ToString()));
            return await client.GetFollowerCountsAsync(request);
        }

        public static async Task<Result<GetFollowingCountsResponse>> GetFollowingCountsAsync(
            this IFollowDataClient client,
            IEnumerable<Guid> userIds)
        {
            var request = new GetFollowingCountsRequest();
            request.UserIds.AddRange(userIds.Select(id => id.ToString()));
            return await client.GetFollowingCountsAsync(request);
        }

        public static async Task<Result> DeleteFollowAsync(
            this IFollowDataClient client,
            Guid id)
        {
            return await client.DeleteFollowAsync(new DeleteFollowRequest { Id = id.ToString() });
        }
    }
}
