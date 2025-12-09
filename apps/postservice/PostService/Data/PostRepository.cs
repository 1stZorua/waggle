using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Waggle.Common.Pagination.Core;
using Waggle.Common.Pagination.Models;
using Waggle.PostService.Models;

namespace Waggle.PostService.Data
{
    public class PostRepository : IPostRepository
    {
        private readonly PostDbContext _context;

        public PostRepository(PostDbContext context) => _context = context;

        public async Task<PagedResult<Post>> GetAllPostsAsync(PaginationRequest request)
        {
            var sortFields = new (Expression<Func<Post, object>> SortBy, string Name)[]
            {
                (u => u.CreatedAt, nameof(Post.CreatedAt)),
                (u => u.Id, nameof(Post.Id))
            };

            return await _context.Posts.AsNoTracking().ToPagedAsync(sortFields, request);
        }

        public async Task<Post?> GetPostByIdAsync(Guid id)
        {
            return await _context.Posts.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddPostAsync(Post post)
        {
            ArgumentNullException.ThrowIfNull(post);

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePostAsync(Post post)
        {
            ArgumentNullException.ThrowIfNull(post);

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }
}
