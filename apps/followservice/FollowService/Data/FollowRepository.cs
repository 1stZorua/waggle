using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Waggle.Common.Pagination.Core;
using Waggle.Common.Pagination.Models;
using Waggle.FollowService.Models;

namespace Waggle.FollowService.Data
{
    public class FollowRepository : IFollowRepository
    {
        private readonly FollowDbContext _context;

        public FollowRepository(FollowDbContext context) => _context = context;

        private static readonly (Expression<Func<Follow, object>> SortBy, string Name)[] SortFields =
        {
            (f => f.CreatedAt, nameof(Follow.CreatedAt)),
            (f => f.Id, nameof(Follow.Id))
        };

        public async Task<PagedResult<Follow>> GetFollowsAsync(
            Guid? followerId = null,
            Guid? followingId = null,
            PaginationRequest request = null!)
        {
            var query = _context.Follows
                .AsNoTracking()
                .AsQueryable();

            if (followerId.HasValue)
                query = query.Where(f => f.FollowerId == followerId.Value);

            if (followingId.HasValue)
                query = query.Where(f => f.FollowingId == followingId.Value);

            query = query
                .OrderByDescending(f => f.CreatedAt)
                .ThenByDescending(f => f.Id);

            return await query.ToPagedAsync(SortFields, request);
        }

        public async Task<Follow?> GetFollowByIdAsync(Guid id)
        {
            return await _context.Follows
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Follow?> GetFollowByFollowerAndFollowingAsync(Guid followerId, Guid followingId)
        {
            return await _context.Follows
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
        }

        public async Task<int> GetFollowerCountAsync(Guid userId)
        {
            return await _context.Follows
                .AsNoTracking()
                .CountAsync(f => f.FollowingId == userId);
        }

        public async Task<int> GetFollowingCountAsync(Guid userId)
        {
            return await _context.Follows
                .AsNoTracking()
                .CountAsync(f => f.FollowerId == userId);
        }

        public async Task AddFollowAsync(Follow follow)
        {
            ArgumentNullException.ThrowIfNull(follow);
            await _context.Follows.AddAsync(follow);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFollowAsync(Follow follow)
        {
            ArgumentNullException.ThrowIfNull(follow);
            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFollowsAsync(Guid? followerId = null, Guid? followingId = null)
        {
            var query = _context.Follows.AsQueryable();

            if (followerId.HasValue)
                query = query.Where(f => f.FollowerId == followerId.Value);

            if (followingId.HasValue)
                query = query.Where(f => f.FollowingId == followingId.Value);

            await query.ExecuteDeleteAsync();
        }
    }
}