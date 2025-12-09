using Waggle.Common.Auth;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.PostService.Dtos;

namespace Waggle.PostService.Services
{
    public interface IPostService
    {
        Task<Result<PagedResult<PostDto>>> GetAllPostsAsync(PaginationRequest request);
        Task<Result<PostDto>> GetPostByIdAsync(Guid id);
        Task<Result<PostDto>> CreatePostAsync(PostCreateDto request, UserInfoDto currentUser);
        Task<Result> DeletePostAsync(Guid id, UserInfoDto currentUser);
    }
}
