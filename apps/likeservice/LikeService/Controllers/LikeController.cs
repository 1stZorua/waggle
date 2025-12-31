using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waggle.Common.Controllers;
using Waggle.Common.Models;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Extensions;
using Waggle.LikeService.Dtos;
using Waggle.LikeService.Services;

namespace Waggle.LikeService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : BaseController
    {
        private readonly ILikeService _service;

        public LikeController(ILikeService service) => _service = service;

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<LikeDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLikes([FromQuery] PaginationRequest request)
        {
            var result = await _service.GetLikesAsync(request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<LikeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLikeById(Guid id)
        {
            var result = await _service.GetLikeByIdAsync(id);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("target/{targetId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<LikeDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLikesByTarget(
            Guid targetId,
            [FromQuery] PaginationRequest request = null!)
        {
            var result = await _service.GetLikesByTargetAsync(targetId, request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<LikeDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLikesByUserId(Guid userId, [FromQuery] PaginationRequest request)
        {
            var result = await _service.GetLikesByUserIdAsync(userId, request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("check/{userId}/{targetId}")]
        [ProducesResponseType(typeof(ApiResponse<LikeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> HasLiked(Guid userId, Guid targetId)
        {
            var result = await _service.HasLikedAsync(userId, targetId);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<LikeDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateLike([FromBody] LikeCreateDto request)
        {
            var result = await _service.CreateLikeAsync(request, CurrentUser);
            return result.ToCreatedResult($"/api/like/{result.Data?.Id}");
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteLike(Guid id)
        {
            var result = await _service.DeleteLikeAsync(id, CurrentUser);
            return result.ToActionResult();
        }
    }
}