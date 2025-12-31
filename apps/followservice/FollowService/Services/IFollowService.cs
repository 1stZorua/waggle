using Waggle.Common.Auth;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.FollowService.Dtos;

namespace Waggle.FollowService.Services
{
    public interface IFollowService : IFollowEventHandler
    {
        Task<Result<PagedResult<FollowDto>>> GetFollowsAsync(PaginationRequest request);
        Task<Result<FollowDto>> GetFollowByIdAsync(Guid id);
        Task<Result<PagedResult<FollowDto>>> GetFollowersAsync(Guid userId, PaginationRequest request);
        Task<Result<PagedResult<FollowDto>>> GetFollowingAsync(Guid userId, PaginationRequest request);
        Task<Result<Dictionary<Guid, int>>> GetFollowerCountsAsync(IEnumerable<Guid> userIds);
        Task<Result<Dictionary<Guid, int>>> GetFollowingCountsAsync(IEnumerable<Guid> userIds);
        Task<Result<FollowDto>> IsFollowingAsync(Guid followerId, Guid followingId);
        Task<Result<FollowDto>> CreateFollowAsync(FollowCreateDto request, UserInfoDto currentUser);
        Task<Result> DeleteFollowAsync(Guid id, UserInfoDto currentUser);
    }
}
