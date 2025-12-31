using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waggle.Common.Controllers;
using Waggle.Common.Models;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Extensions;
using Waggle.CommentService.Dtos;
using Waggle.CommentService.Services;

namespace Waggle.CommentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : BaseController
    {
        private readonly ICommentService _service;

        public CommentController(ICommentService service) => _service = service;

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CommentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComments([FromQuery] PaginationRequest request)
        {
            var result = await _service.GetCommentsAsync(request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CommentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            var result = await _service.GetCommentByIdAsync(id);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("posts/{postId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CommentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCommentsByPost(Guid postId, [FromQuery] PaginationRequest request = null!)
        {
            var result = await _service.GetCommentsByPostAsync(postId, request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{commentId}/replies")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CommentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReplies(
            Guid commentId,
            [FromQuery] PaginationRequest request = null!)
        {
            var result = await _service.GetRepliesAsync(commentId, request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CommentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCommentsByUser(Guid userId, [FromQuery] PaginationRequest request = null!)
        {
            var result = await _service.GetCommentsByUserAsync(userId, request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CommentDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateComment([FromBody] CommentCreateDto request)
        {
            var result = await _service.CreateCommentAsync(request, CurrentUser);
            return result.ToCreatedResult($"/api/comment/{result.Data?.Id}");
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CommentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] CommentUpdateDto request)
        {
            var result = await _service.UpdateCommentAsync(id, request, CurrentUser);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var result = await _service.DeleteCommentAsync(id, CurrentUser);
            return result.ToActionResult();
        }
    }
}