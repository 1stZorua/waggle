using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Waggle.Common.Pagination.Core;
using Waggle.Common.Pagination.Models;
using Waggle.MediaService.Dtos;
using Waggle.MediaService.Models;

namespace Waggle.MediaService.Data
{
    public class MediaRepository : IMediaRepository
    {
        private readonly MediaDbContext _context;

        public MediaRepository(MediaDbContext context) => _context = context;

        public async Task<PagedResult<Media>> GetAllMediaAsync(PaginationRequest request)
        {
            var sortFields = new (Expression<Func<Media, object>> SortBy, string Name)[]
            {
                (u => u.CreatedAt, nameof(Media.CreatedAt)),
                (u => u.Id, nameof(Media.Id))
            };

            return await _context.Media.AsNoTracking().ToPagedAsync(sortFields, request);
        }

        public async Task<Media?> GetMediaByIdAsync(Guid id)
        {
            return await _context.Media.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<Media>> GetMediaByIdsAsync(MediaBatchRequest request)
        {
            var ids = request.Ids?.ToList() ?? [];
            if (ids.Count == 0) return [];

            return await _context.Media
                .AsNoTracking()
                .Where(u => ids.Contains(u.Id))
                .ToListAsync();
        }

        public async Task AddMediaAsync(Media media)
        {
            ArgumentNullException.ThrowIfNull(media);

            await _context.Media.AddAsync(media);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMediaAsync(Media media)
        {
            ArgumentNullException.ThrowIfNull(media);

            _context.Media.Remove(media);
            await _context.SaveChangesAsync();
        }
    }
}
