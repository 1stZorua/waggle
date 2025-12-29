using Waggle.Common.Results.Core;
using Waggle.Contracts.Post.Grpc;

namespace Waggle.Contracts.Post.Interfaces
{
    public interface IPostDataClient
    {
        Task<Result<GetPostsResponse>> GetPostsAsync(GetPostsRequest request);
        Task<Result<GetPostByIdResponse>> GetPostByIdAsync(GetPostByIdRequest request);
        Task<Result<GetPostsByUserIdResponse>> GetPostsByUserIdAsync(GetPostsByUserIdRequest request);
        Task<Result<CreatePostResponse>> CreatePostAsync(CreatePostRequest request);
        Task<Result> DeletePostAsync(DeletePostRequest request);
    }
}
