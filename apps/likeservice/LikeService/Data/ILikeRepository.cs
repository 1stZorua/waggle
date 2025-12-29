using Waggle.Common.Models;
using Waggle.Common.Pagination.Models;
using Waggle.LikeService.Models;

namespace Waggle.LikeService.Data
{
    public interface ILikeRepository
    {
        Task<PagedResult<Like>> GetLikesAsync(
            Guid? targetId = null,
            InteractionType? targetType = null,
            Guid? userId = null,
            PaginationRequest request = null!);
        Task<Like?> GetLikeByIdAsync(Guid id);
        Task<Like?> GetLikeByUserAndTargetAsync(Guid userId, Guid targetId);
        Task<int> GetLikeCountAsync(Guid targetId);
        Task AddLikeAsync(Like like);
        Task DeleteLikeAsync(Like like);
        Task DeleteLikesAsync(Guid? targetId = null, InteractionType? targetType = null, Guid? userId = null);
    }
}