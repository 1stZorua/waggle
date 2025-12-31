using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Waggle.Common.Models;
using Waggle.Common.Pagination.Core;
using Waggle.Common.Pagination.Models;
using Waggle.LikeService.Models;

namespace Waggle.LikeService.Data
{
    public class LikeRepository : ILikeRepository
    {
        private readonly LikeDbContext _context;

        public LikeRepository(LikeDbContext context) => _context = context;

        private static readonly (Expression<Func<Like, object>> SortBy, string Name)[] SortFields =
        {
            (l => l.CreatedAt, nameof(Like.CreatedAt)),
            (l => l.Id, nameof(Like.Id))
        };

        public async Task<PagedResult<Like>> GetLikesAsync(
            Guid? targetId = null,
            InteractionType? targetType = null,
            Guid? userId = null,
            PaginationRequest request = null!)
        {
            var query = _context.Likes
                .AsNoTracking()
                .AsQueryable();

            if (targetId.HasValue)
                query = query.Where(l => l.TargetId == targetId.Value);

            if (targetType.HasValue)
                query = query.Where(l => l.TargetType == targetType.Value);

            if (userId.HasValue)
                query = query.Where(l => l.UserId == userId.Value);

            query = query
                .OrderByDescending(l => l.CreatedAt)
                .ThenByDescending(l => l.Id);

            return await query.ToPagedAsync(SortFields, request);
        }

        public async Task<Like?> GetLikeByIdAsync(Guid id)
        {
            return await _context.Likes
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Like?> GetLikeByUserAndTargetAsync(Guid userId, Guid targetId)
        {
            return await _context.Likes
                .AsNoTracking()
                .FirstOrDefaultAsync(l =>
                    l.UserId == userId &&
                    l.TargetId == targetId);
        }

        public async Task<Dictionary<Guid, int>> GetLikeCountsAsync(IEnumerable<Guid> targetIds)
        {
            var ids = targetIds.Distinct().ToList();
            if (ids.Count == 0)
                return [];

            return await _context.Likes
                .AsNoTracking()
                .Where(l => ids.Contains(l.TargetId))
                .GroupBy(l => l.TargetId)
                .Select(g => new { TargetId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TargetId, x => x.Count);
        }

        public async Task AddLikeAsync(Like like)
        {
            ArgumentNullException.ThrowIfNull(like);
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLikeAsync(Like like)
        {
            ArgumentNullException.ThrowIfNull(like);
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLikesAsync(
            Guid? targetId = null,
            InteractionType? targetType = null,
            Guid? userId = null)
        {
            var query = _context.Likes.AsQueryable();

            if (targetId.HasValue)
                query = query.Where(l => l.TargetId == targetId.Value);

            if (targetType.HasValue)
                query = query.Where(l => l.TargetType == targetType.Value);

            if (userId.HasValue)
                query = query.Where(l => l.UserId == userId.Value);

            await query.ExecuteDeleteAsync();
        }
    }
}