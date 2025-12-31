using Waggle.Common.Results.Core;
using Waggle.Contracts.Comment.Grpc;
using Waggle.Contracts.Comment.Interfaces;

namespace Waggle.Contracts.Comment.Extensions
{
    public static class CommentDataClientExtensions
    {
        public static async Task<Result<GetCommentByIdResponse>> GetCommentByIdAsync(
            this ICommentDataClient client,
            Guid id)
        {
            return await client.GetCommentByIdAsync(new GetCommentByIdRequest { Id = id.ToString() });
        }

        public static async Task<Result> DeleteCommentAsync(
            this ICommentDataClient client,
            Guid id)
        {
            return await client.DeleteCommentAsync(new DeleteCommentRequest { Id = id.ToString() });
        }

        public static async Task<Result<GetCommentCountsResponse>> GetCommentCountsAsync(
            this ICommentDataClient client,
            IEnumerable<Guid> postIds)
        {
            var request = new GetCommentCountsRequest();
            request.PostIds.AddRange(postIds.Select(id => id.ToString()));
            return await client.GetCommentCountsAsync(request);
        }

        public static async Task<Result<GetReplyCountsResponse>> GetReplyCountsAsync(
            this ICommentDataClient client,
            IEnumerable<Guid> commentIds)
        {
            var request = new GetReplyCountsRequest();
            request.CommentIds.AddRange(commentIds.Select(id => id.ToString()));
            return await client.GetReplyCountsAsync(request);
        }
    }
}
