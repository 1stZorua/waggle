using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Waggle.Common.Models;
using Waggle.Common.Pagination.Core;
using Waggle.Common.Pagination.Models;
using Waggle.FavoriteService.Models;
using Waggle.FollowService.Data;

namespace Waggle.FavoriteService.Data
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly FavoriteDbContext _context;

        public FavoriteRepository(FavoriteDbContext context) => _context = context;

        private static readonly (Expression<Func<Favorite, object>> SortBy, string Name)[] SortFields =
        {
            (f => f.CreatedAt, nameof(Favorite.CreatedAt)),
            (f => f.Id, nameof(Favorite.Id))
        };

        public async Task<PagedResult<Favorite>> GetFavoritesAsync(
            Guid? targetId = null,
            InteractionType? targetType = null,
            Guid? userId = null,
            PaginationRequest request = null!)
        {
            var query = _context.Favorites
                .AsNoTracking()
                .AsQueryable();

            if (targetId.HasValue)
                query = query.Where(f => f.TargetId == targetId.Value);

            if (targetType.HasValue)
                query = query.Where(f => f.TargetType == targetType.Value);

            if (userId.HasValue)
                query = query.Where(f => f.UserId == userId.Value);

            query = query
                .OrderByDescending(f => f.CreatedAt)
                .ThenByDescending(f => f.Id);

            return await query.ToPagedAsync(SortFields, request);
        }

        public async Task<Favorite?> GetFavoriteByIdAsync(Guid id)
        {
            return await _context.Favorites
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Favorite?> GetFavoriteByUserAndTargetAsync(Guid userId, Guid targetId)
        {
            return await _context.Favorites
                .AsNoTracking()
                .FirstOrDefaultAsync(f =>
                    f.UserId == userId &&
                    f.TargetId == targetId);
        }

        public async Task<Dictionary<Guid, int>> GetFavoriteCountsAsync(IEnumerable<Guid> targetIds)
        {
            var ids = targetIds.Distinct().ToList();
            if (ids.Count == 0)
                return new Dictionary<Guid, int>();

            return await _context.Favorites
                .AsNoTracking()
                .Where(f => ids.Contains(f.TargetId))
                .GroupBy(f => f.TargetId)
                .Select(g => new { TargetId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TargetId, x => x.Count);
        }

        public async Task AddFavoriteAsync(Favorite favorite)
        {
            ArgumentNullException.ThrowIfNull(favorite);
            await _context.Favorites.AddAsync(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFavoriteAsync(Favorite favorite)
        {
            ArgumentNullException.ThrowIfNull(favorite);
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFavoritesAsync(
            Guid? targetId = null,
            InteractionType? targetType = null,
            Guid? userId = null)
        {
            var query = _context.Favorites.AsQueryable();

            if (targetId.HasValue)
                query = query.Where(f => f.TargetId == targetId.Value);

            if (targetType.HasValue)
                query = query.Where(f => f.TargetType == targetType.Value);

            if (userId.HasValue)
                query = query.Where(f => f.UserId == userId.Value);

            await query.ExecuteDeleteAsync();
        }
    }
}