using Waggle.Common.Auth;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.MediaService.Dtos;

namespace Waggle.MediaService.Services
{
    public interface IMediaService
    {
        Task<Result<PagedResult<MediaDto>>> GetAllMediaAsync(PaginationRequest request);
        Task<Result<MediaDto>> GetMediaByIdAsync(Guid id);
        Task<Result<List<MediaDto>>> GetMediaByIdsAsync(MediaBatchRequest request);
        Task<Result<UrlResponseDto>> GetPresignedMediaUrlAsync(Guid id);
        Task<Result<Dictionary<Guid, UrlResponseDto>>> GetPresignedMediaUrlsAsync(MediaBatchRequest request);
        Task<Result<MediaDto>> UploadMediaAsync(MediaCreateDto request, UserInfoDto currentUser);
        Task<Result> DeleteMediaAsync(Guid id, UserInfoDto currentUser);
    }
}
