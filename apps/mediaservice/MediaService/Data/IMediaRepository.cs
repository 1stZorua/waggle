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
        Task AddMediaAsync(Media media);
        Task DeleteMediaAsync(Media media);
    }   
}
