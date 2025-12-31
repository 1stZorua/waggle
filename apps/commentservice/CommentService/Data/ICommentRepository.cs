using Waggle.Common.Pagination.Models;
using Waggle.CommentService.Models;

namespace Waggle.CommentService.Data
{
    public interface ICommentRepository
    {
        Task<PagedResult<Comment>> GetCommentsAsync(
            Guid? postId = null,
            Guid? parentId = null,
            Guid? userId = null,
            PaginationRequest request = null!);
        Task<Comment?> GetCommentByIdAsync(Guid id);
        Task<Dictionary<Guid, int>> GetCommentCountsAsync(IEnumerable<Guid> postIds);
        Task<Dictionary<Guid, int>> GetReplyCountsAsync(IEnumerable<Guid> commentIds);
        Task<List<Guid>> GetAllReplyIdsRecursivelyAsync(Guid commentId);
        Task AddCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(Comment comment);
        Task DeleteCommentsAsync(Guid? postId = null, Guid? parentId = null, Guid? userId = null);
        Task DeleteCommentsByIdsAsync(IEnumerable<Guid> commentIds);
    }
}