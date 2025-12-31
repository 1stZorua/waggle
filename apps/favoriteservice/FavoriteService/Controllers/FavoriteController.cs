using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waggle.Common.Auth;
using Waggle.Common.Models;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Extensions;
using Waggle.FavoriteService.Dtos;
using Waggle.FavoriteService.Services;

namespace Waggle.FavoriteService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _service;

        public FavoriteController(IFavoriteService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<FavoriteDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFavorites([FromQuery] PaginationRequest request)
        {
            var result = await _service.GetFavoritesAsync(request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<FavoriteDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFavoriteById(Guid id)
        {
            var result = await _service.GetFavoriteByIdAsync(id);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("target/{targetId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<FavoriteDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFavoritesByTarget(
            Guid targetId,
            [FromQuery] PaginationRequest request = null!)
        {
            var result = await _service.GetFavoritesByTargetAsync(targetId, request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<FavoriteDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFavoritesByUserId(Guid userId, [FromQuery] PaginationRequest request)
        {
            var result = await _service.GetFavoritesByUserIdAsync(userId, request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("check/{userId}/{targetId}")]
        [ProducesResponseType(typeof(ApiResponse<FavoriteDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> HasFavorited(Guid userId, Guid targetId)
        {
            var result = await _service.HasFavoritedAsync(userId, targetId);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<FavoriteDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateFavorite([FromBody] FavoriteCreateDto request)
        {
            var currentUser = User.ToUserInfo();
            var result = await _service.CreateFavoriteAsync(request, currentUser);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteFavorite(Guid id)
        {
            var currentUser = User.ToUserInfo();
            var result = await _service.DeleteFavoriteAsync(id, currentUser);
            return result.ToActionResult();
        }
    }
}