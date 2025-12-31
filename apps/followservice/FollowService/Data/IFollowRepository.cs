using Waggle.Common.Pagination.Models;
using Waggle.FollowService.Models;

namespace Waggle.FollowService.Data
{
    public interface IFollowRepository
    {
        Task<PagedResult<Follow>> GetFollowsAsync(
            Guid? followerId = null,
            Guid? followingId = null,
            PaginationRequest request = null!);
        Task<Follow?> GetFollowByIdAsync(Guid id);
        Task<Follow?> GetFollowByFollowerAndFollowingAsync(Guid followerId, Guid followingId);
        Task<Dictionary<Guid, int>> GetFollowerCountsAsync(IEnumerable<Guid> userIds);
        Task<Dictionary<Guid, int>> GetFollowingCountsAsync(IEnumerable<Guid> userIds);
        Task AddFollowAsync(Follow follow);
        Task DeleteFollowAsync(Follow follow);
        Task DeleteFollowsAsync(Guid? followerId = null, Guid? followingId = null);
    }
}
