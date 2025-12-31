using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using Waggle.Common.Pagination.Core;
using Waggle.Common.Pagination.Models;
using Waggle.PostService.Models;

namespace Waggle.PostService.Data
{
    public class PostRepository : IPostRepository
    {
        private readonly PostDbContext _context;

        public PostRepository(PostDbContext context) => _context = context;

        public async Task<PagedResult<Post>> GetPostsAsync(PaginationRequest request)
        {
            var sortFields = new (Expression<Func<Post, object>> SortBy, string Name)[]
            {
                (u => u.CreatedAt, nameof(Post.CreatedAt)),
                (u => u.Id, nameof(Post.Id))
            };

            var query = _context.Posts.AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.Id);

            return await query.ToPagedAsync(sortFields, request);
        }

        public async Task<Post?> GetPostByIdAsync(Guid id)
        {
            return await _context.Posts.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<PagedResult<Post>> GetPostsByUserIdAsync(Guid userId, PaginationRequest request)
        {
            var sortFields = new (Expression<Func<Post, object>> SortBy, string Name)[]
            {
                (u => u.CreatedAt, nameof(Post.CreatedAt)),
                (u => u.Id, nameof(Post.Id))
            };

            var query = _context.Posts.AsNoTracking()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt);

            return await query.ToPagedAsync(sortFields, request);
        }

        public async Task<Dictionary<Guid, int>> GetPostCountsAsync(IEnumerable<Guid> userIds)
        {
            var ids = userIds.Distinct().ToList();
            if (ids.Count == 0)
                return [];

            return await _context.Posts
                .AsNoTracking()
                .Where(p => ids.Contains(p.UserId))
                .GroupBy(p => p.UserId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count);
        }

        public async Task AddPostAsync(Post post)
        {
            ArgumentNullException.ThrowIfNull(post);

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePostAsync(Post post)
        {
            ArgumentNullException.ThrowIfNull(post);

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePostAsync(Post post)
        {
            ArgumentNullException.ThrowIfNull(post);

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePostsByUserIdAsync(Guid userId)
        {
            await _context.Posts
                .Where(p => p.UserId == userId)
                .ExecuteDeleteAsync();
        }
    }
}
