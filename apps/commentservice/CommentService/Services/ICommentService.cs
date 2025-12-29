using Waggle.Common.Auth;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.CommentService.Dtos;

namespace Waggle.CommentService.Services
{
    public interface ICommentService
    {
        Task<Result<PagedResult<CommentDto>>> GetCommentsAsync(PaginationRequest request);
        Task<Result<CommentDto>> GetCommentByIdAsync(Guid id);
        Task<Result<PagedResult<CommentDto>>> GetCommentsByPostAsync(Guid postId, PaginationRequest request);
        Task<Result<PagedResult<CommentDto>>> GetRepliesAsync(Guid commentId, PaginationRequest request);
        Task<Result<PagedResult<CommentDto>>> GetCommentsByUserAsync(Guid userId, PaginationRequest request);
        Task<Result<int>> GetCommentCountAsync(Guid postId);
        Task<Result<int>> GetReplyCountAsync(Guid commentId);
        Task<Result<CommentDto>> CreateCommentAsync(CommentCreateDto request, UserInfoDto currentUser);
        Task<Result<CommentDto>> UpdateCommentAsync(Guid id, CommentUpdateDto request, UserInfoDto currentUser);
        Task<Result> DeleteCommentAsync(Guid id, UserInfoDto currentUser);
    }
}