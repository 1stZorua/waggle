using Waggle.Common.Auth;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.FavoriteService.Dtos;

namespace Waggle.FavoriteService.Services
{
    public interface IFavoriteService
    {
        Task<Result<PagedResult<FavoriteDto>>> GetFavoritesAsync(PaginationRequest request);
        Task<Result<FavoriteDto>> GetFavoriteByIdAsync(Guid id);
        Task<Result<PagedResult<FavoriteDto>>> GetFavoritesByTargetAsync(Guid targetId, PaginationRequest request);
        Task<Result<PagedResult<FavoriteDto>>> GetFavoritesByUserIdAsync(Guid userId, PaginationRequest request);
        Task<Result<int>> GetFavoriteCountAsync(Guid targetId);
        Task<Result<FavoriteDto>> HasFavoritedAsync(Guid userId, Guid targetId);
        Task<Result<FavoriteDto>> CreateFavoriteAsync(FavoriteCreateDto request, UserInfoDto currentUser);
        Task<Result> DeleteFavoriteAsync(Guid id, UserInfoDto currentUser);
    }
}