using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waggle.Common.Controllers;
using Waggle.Common.Models;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Extensions;
using Waggle.PostService.Dtos;
using Waggle.PostService.Services;

namespace Waggle.PostService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : BaseController
    {
        private readonly IPostService _service;

        public PostController(IPostService service) => _service = service;

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PostDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPosts([FromQuery] PaginationRequest request)
        {
            var result = await _service.GetPostsAsync(request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PostDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var result = await _service.GetPostByIdAsync(id);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("users/{userId}/posts")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PostDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPostsByUserId(Guid userId, [FromQuery] PaginationRequest request)
        {
            var result = await _service.GetPostsByUserIdAsync(userId, request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PostDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePost(PostCreateDto request)
        {
            var result = await _service.CreatePostAsync(request, CurrentUser);
            return result.ToCreatedResult($"/api/posts/{result.Data?.Id}");
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PostDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePost(Guid id, PostUpdateDto request)
        {
            var result = await _service.UpdatePostAsync(id, request, CurrentUser);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var result = await _service.DeletePostAsync(id, CurrentUser);
            return result.ToActionResult();
        }
    }
}