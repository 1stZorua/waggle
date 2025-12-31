using Waggle.Common.Pagination.Models;
using Waggle.PostService.Models;

namespace Waggle.PostService.Data
{
    public interface IPostRepository
    {
        Task<PagedResult<Post>> GetPostsAsync(PaginationRequest request);
        Task<Post?> GetPostByIdAsync(Guid id);
        Task<PagedResult<Post>> GetPostsByUserIdAsync(Guid userId, PaginationRequest request);
        Task<Dictionary<Guid, int>> GetPostCountsAsync(IEnumerable<Guid> userIds);
        Task AddPostAsync(Post post);
        Task UpdatePostAsync(Post post);
        Task DeletePostAsync(Post post);
        Task DeletePostsByUserIdAsync(Guid userId);
    }   
}
