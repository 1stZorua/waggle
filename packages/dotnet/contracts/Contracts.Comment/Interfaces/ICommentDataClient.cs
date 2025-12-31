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
        Task<Result<GetCommentCountsResponse>> GetCommentCountsAsync(GetCommentCountsRequest request);
        Task<Result<GetReplyCountsResponse>> GetReplyCountsAsync(GetReplyCountsRequest request);
        Task<Result<CreateCommentResponse>> CreateCommentAsync(CreateCommentRequest request);
        Task<Result<UpdateCommentResponse>> UpdateCommentAsync(UpdateCommentRequest request); // Changed from UpdateCommentResponse
        Task<Result> DeleteCommentAsync(DeleteCommentRequest request);
    }
}