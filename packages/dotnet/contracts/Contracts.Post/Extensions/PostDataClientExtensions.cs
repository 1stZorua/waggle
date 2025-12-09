using Waggle.Common.Results.Core;
using Waggle.Contracts.Post.Interfaces;
using Waggle.Contracts.Post.Grpc;

namespace Waggle.Contracts.Post.Extensions
{
    public static class PostDataClientExtensions
    {
        public static async Task<Result<GetPostByIdResponse>> GetPostByIdAsync(
            this IPostDataClient client,
            Guid id)
        {
            return await client.GetPostByIdAsync(new GetPostByIdRequest { Id = id.ToString() });
        }

        public static async Task<Result> DeletePostAsync(
            this IPostDataClient client,
            Guid id)
        {
            return await client.DeletePostAsync(new DeletePostRequest { Id = id.ToString() });
        }
    }
}
