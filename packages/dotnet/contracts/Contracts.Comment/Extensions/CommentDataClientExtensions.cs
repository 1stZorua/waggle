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

        public static async Task<Result<GetCommentCountResponse>> GetCommentCountAsync(
            this ICommentDataClient client,
            Guid postId)
        {
            return await client.GetCommentCountAsync(new GetCommentCountRequest { PostId = postId.ToString() });
        }
    }
}
