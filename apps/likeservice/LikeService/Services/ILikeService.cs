using Waggle.Common.Auth;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.LikeService.Dtos;
using Waggle.LikeService.Models;

namespace Waggle.LikeService.Services
{
    public interface ILikeService
    {
        Task<Result<PagedResult<LikeDto>>> GetLikesAsync(PaginationRequest request);
        Task<Result<LikeDto>> GetLikeByIdAsync(Guid id);
        Task<Result<PagedResult<LikeDto>>> GetLikesByTargetAsync(Guid targetId, PaginationRequest request);
        Task<Result<PagedResult<LikeDto>>> GetLikesByUserIdAsync(Guid userId, PaginationRequest request);
        Task<Result<int>> GetLikeCountAsync(Guid targetId);
        Task<Result<LikeDto>> HasLikedAsync(Guid userId, Guid targetId);
        Task<Result<LikeDto>> CreateLikeAsync(LikeCreateDto request, UserInfoDto currentUser);
        Task<Result> DeleteLikeAsync(Guid id, UserInfoDto currentUser);
    }
}
