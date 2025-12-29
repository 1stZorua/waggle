using Waggle.Common.Models;
using Waggle.Common.Pagination.Models;
using Waggle.FavoriteService.Models;

namespace Waggle.FollowService.Data
{
    public interface IFavoriteRepository
    {
        Task<PagedResult<Favorite>> GetFavoritesAsync(
            Guid? targetId = null,
            InteractionType? targetType = null,
            Guid? userId = null,
            PaginationRequest request = null!);
        Task<Favorite?> GetFavoriteByIdAsync(Guid id);
        Task<Favorite?> GetFavoriteByUserAndTargetAsync(Guid userId, Guid targetId);
        Task<int> GetFavoriteCountAsync(Guid targetId);
        Task AddFavoriteAsync(Favorite favorite);
        Task DeleteFavoriteAsync(Favorite favorite);
        Task DeleteFavoritesAsync(Guid? targetId = null, InteractionType? targetType = null, Guid? userId = null);
    }
}
