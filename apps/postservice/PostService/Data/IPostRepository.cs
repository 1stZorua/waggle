using Waggle.Common.Pagination.Models;
using Waggle.PostService.Models;

namespace Waggle.PostService.Data
{
    public interface IPostRepository
    {
        Task<PagedResult<Post>> GetAllPostsAsync(PaginationRequest request);
        Task<Post?> GetPostByIdAsync(Guid id);
        Task AddPostAsync(Post post);
        Task DeletePostAsync(Post post);
    }   
}
