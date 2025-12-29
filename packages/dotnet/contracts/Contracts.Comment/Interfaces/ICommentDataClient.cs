using Waggle.Common.Results.Core;
using Waggle.Contracts.Comment.Grpc;

namespace Waggle.Contracts.Comment.Interfaces
{
    public interface ICommentDataClient
    {
        Task<Result<GetCommentsResponse>> GetCommentsAsync(GetCommentsRequest request);
        Task<Result<GetCommentByIdResponse>> GetCommentByIdAsync(GetCommentByIdRequest request);
        Task<Result<GetCommentsByPostResponse>> GetCommentsByPostAsync(GetCommentsByPostRequest request);
        Task<Result<GetRepliesResponse>> GetRepliesAsync(GetRepliesRequest request);
        Task<Result<GetCommentsByUserResponse>> GetCommentsByUserAsync(GetCommentsByUserRequest request);
        Task<Result<GetCommentCountResponse>> GetCommentCountAsync(GetCommentCountRequest request);
        Task<Result<GetReplyCountResponse>> GetReplyCountAsync(GetReplyCountRequest request);
        Task<Result<CreateCommentResponse>> CreateCommentAsync(CreateCommentRequest request);
        Task<Result<UpdateCommentResponse>> UpdateCommentAsync(UpdateCommentRequest request);
        Task<Result> DeleteCommentAsync(DeleteCommentRequest request);
    }
}