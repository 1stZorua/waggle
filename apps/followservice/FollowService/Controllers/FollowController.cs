using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waggle.Common.Auth;
using Waggle.Common.Models;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Extensions;
using Waggle.FollowService.Dtos;
using Waggle.FollowService.Services;

namespace Waggle.FollowService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _service;

        public FollowController(IFollowService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<FollowDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFollows([FromQuery] PaginationRequest request)
        {
            var result = await _service.GetFollowsAsync(request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<FollowDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFollowById(Guid id)
        {
            var result = await _service.GetFollowByIdAsync(id);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("followers/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<FollowDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFollowers(Guid userId, [FromQuery] PaginationRequest request)
        {
            var result = await _service.GetFollowersAsync(userId, request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("following/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<FollowDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFollowing(Guid userId, [FromQuery] PaginationRequest request)
        {
            var result = await _service.GetFollowingAsync(userId, request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("check/{followerId}/{followingId}")]
        [ProducesResponseType(typeof(ApiResponse<FollowDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> IsFollowing(Guid followerId, Guid followingId)
        {
            var result = await _service.IsFollowingAsync(followerId, followingId);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<FollowDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateFollow([FromBody] FollowCreateDto request)
        {
            var currentUser = User.ToUserInfo();
            var result = await _service.CreateFollowAsync(request, currentUser);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteFollow(Guid id)
        {
            var currentUser = User.ToUserInfo();
            var result = await _service.DeleteFollowAsync(id, currentUser);
            return result.ToActionResult();
        }
    }
}