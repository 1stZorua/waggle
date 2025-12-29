using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Waggle.Common.Pagination.Core;
using Waggle.Common.Pagination.Models;
using Waggle.CommentService.Models;

namespace Waggle.CommentService.Data
{
    public class CommentRepository : ICommentRepository
    {
        private readonly CommentDbContext _context;

        public CommentRepository(CommentDbContext context) => _context = context;

        private static readonly (Expression<Func<Comment, object>> SortBy, string Name)[] SortFields =
        {
            (c => c.CreatedAt, nameof(Comment.CreatedAt)),
            (c => c.Id, nameof(Comment.Id))
        };

        public async Task<PagedResult<Comment>> GetCommentsAsync(
            Guid? postId = null,
            Guid? parentCommentId = null,
            Guid? userId = null,
            PaginationRequest request = null!)
        {
            var query = _context.Comments
                .AsNoTracking()
                .AsQueryable();

            if (postId.HasValue)
                query = query.Where(c => c.PostId == postId.Value);

            if (parentCommentId.HasValue)
                query = query.Where(c => c.ParentCommentId == parentCommentId.Value);
            else if (postId.HasValue)
                query = query.Where(c => c.ParentCommentId == null);

            if (userId.HasValue)
                query = query.Where(c => c.UserId == userId.Value);

            query = query
                .OrderByDescending(c => c.CreatedAt)
                .ThenByDescending(c => c.Id);

            return await query.ToPagedAsync(SortFields, request);
        }

        public async Task<Comment?> GetCommentByIdAsync(Guid id)
        {
            return await _context.Comments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> GetCommentCountAsync(Guid postId)
        {
            return await _context.Comments
                .AsNoTracking()
                .CountAsync(c => c.PostId == postId);
        }

        public async Task<int> GetReplyCountAsync(Guid commentId)
        {
            return await _context.Comments
                .AsNoTracking()
                .CountAsync(c => c.ParentCommentId == commentId);
        }

        public async Task AddCommentAsync(Comment comment)
        {
            ArgumentNullException.ThrowIfNull(comment);
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            ArgumentNullException.ThrowIfNull(comment);
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(Comment comment)
        {
            ArgumentNullException.ThrowIfNull(comment);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentsAsync(
            Guid? postId = null,
            Guid? parentCommentId = null,
            Guid? userId = null)
        {
            var query = _context.Comments.AsQueryable();

            if (postId.HasValue)
                query = query.Where(c => c.PostId == postId.Value);

            if (parentCommentId.HasValue)
                query = query.Where(c => c.ParentCommentId == parentCommentId.Value);

            if (userId.HasValue)
                query = query.Where(c => c.UserId == userId.Value);

            await query.ExecuteDeleteAsync();
        }
    }
}