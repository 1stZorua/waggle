using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waggle.Common.Controllers;
using Waggle.Common.Models;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Extensions;
using Waggle.UserService.Dtos;
using Waggle.UserService.Services;

namespace Waggle.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUserService _service;

        public UsersController(IUserService service) => _service = service;

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationRequest request)
        {
            var result = await _service.GetAllUsersAsync(request);
            return result.ToActionResult();
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _service.GetUserByIdAsync(id);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUser(UserCreateDto dto)
        {
            var result = await _service.CreateUserAsync(dto);
            return result.ToCreatedResult($"/api/users/{result.Data?.Id}");
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _service.DeleteUserAsync(id);
            return result.ToActionResult();
        }
    }
}
