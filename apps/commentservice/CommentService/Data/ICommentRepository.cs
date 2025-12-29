using Waggle.Common.Pagination.Models;
using Waggle.CommentService.Models;

namespace Waggle.CommentService.Data
{
    public interface ICommentRepository
    {
        Task<PagedResult<Comment>> GetCommentsAsync(
            Guid? postId = null,
            Guid? parentCommentId = null,
            Guid? userId = null,
            PaginationRequest request = null!);
        Task<Comment?> GetCommentByIdAsync(Guid id);
        Task<int> GetCommentCountAsync(Guid postId);
        Task<int> GetReplyCountAsync(Guid commentId);
        Task AddCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(Comment comment);
        Task DeleteCommentsAsync(Guid? postId = null, Guid? parentCommentId = null, Guid? userId = null);
    }
}