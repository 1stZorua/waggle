using Waggle.Common.Pagination.Models;
using Waggle.MediaService.Dtos;
using Waggle.MediaService.Models;

namespace Waggle.MediaService.Data
{
    public interface IMediaRepository
    {
        Task<PagedResult<Media>> GetAllMediaAsync(PaginationRequest request);
        Task<Media?> GetMediaByIdAsync(Guid id);
        Task<List<Media>> GetMediaByIdsAsync(MediaBatchRequest request);
        Task<List<Media>> GetAllMediaByUploaderIdAsync(Guid uploaderId);
        Task AddMediaAsync(Media media);
        Task AddMediaBatchAsync(IEnumerable<Media> mediaList);
        Task DeleteMediaAsync(Media media);
        Task DeleteAllMediaByUploaderIdAsync(Guid uploaderId);
        Task DeleteAllMediaByIdsAsync(MediaBatchRequest request);
    }   
}
