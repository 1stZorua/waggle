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
            Guid? parentId = null,
            Guid? userId = null,
            PaginationRequest request = null!)
        {
            var query = _context.Comments
                .AsNoTracking()
                .AsQueryable();

            if (postId.HasValue)
                query = query.Where(c => c.PostId == postId.Value);

            if (parentId.HasValue)
                query = query.Where(c => c.ParentId == parentId.Value);
            else if (postId.HasValue)
                query = query.Where(c => c.ParentId == null);

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

        public async Task<Dictionary<Guid, int>> GetCommentCountsAsync(IEnumerable<Guid> postIds)
        {
            var ids = postIds.Distinct().ToList();
            if (ids.Count == 0)
                return [];

            return await _context.Comments
                .AsNoTracking()
                .Where(c => c.ParentId == null && ids.Contains(c.PostId))
                .GroupBy(c => c.PostId)
                .Select(g => new { PostId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PostId, x => x.Count);
        }

        public async Task<Dictionary<Guid, int>> GetReplyCountsAsync(IEnumerable<Guid> commentIds)
        {
            var ids = commentIds.Distinct().ToList();
            if (ids.Count == 0)
                return [];

            return await _context.Comments
                .AsNoTracking()
                .Where(c =>
                    c.ParentId != null &&
                    ids.Contains(c.ParentId.Value))
                .GroupBy(c => c.ParentId!.Value)
                .Select(g => new { CommentId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CommentId, x => x.Count);
        }

        public async Task<List<Guid>> GetAllReplyIdsRecursivelyAsync(Guid commentId)
        {
            var allReplyIds = new List<Guid>();
            var toProcess = new Queue<Guid>();
            toProcess.Enqueue(commentId);

            while (toProcess.Count > 0)
            {
                var currentId = toProcess.Dequeue();

                var directReplyIds = await _context.Comments
                    .AsNoTracking()
                    .Where(c => c.ParentId == currentId)
                    .Select(c => c.Id)
                    .ToListAsync();

                foreach (var replyId in directReplyIds)
                {
                    allReplyIds.Add(replyId);
                    toProcess.Enqueue(replyId);
                }
            }

            return allReplyIds;
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
            Guid? parentId = null,
            Guid? userId = null)
        {
            var query = _context.Comments.AsQueryable();

            if (postId.HasValue)
                query = query.Where(c => c.PostId == postId.Value);

            if (parentId.HasValue)
                query = query.Where(c => c.ParentId == parentId.Value);

            if (userId.HasValue)
                query = query.Where(c => c.UserId == userId.Value);

            await query.ExecuteDeleteAsync();
        }

        public async Task DeleteCommentsByIdsAsync(IEnumerable<Guid> commentIds)
        {
            await _context.Comments
                .Where(c => commentIds.Contains(c.Id))
                .ExecuteDeleteAsync();
        }
    }
}