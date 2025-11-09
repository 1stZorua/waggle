using Microsoft.AspNetCore.Mvc;
using Waggle.Common.Models;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Extensions;
using Waggle.UserService.Dtos;
using Waggle.UserService.Services;

namespace Waggle.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service) => _service = service;

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationRequest request)
        {
            var result = await _service.GetAllUsersAsync(request);
            return result.ToActionResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _service.GetUserByIdAsync(id);
            return result.ToActionResult();
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUser(UserCreateDto dto)
        {
            var result = await _service.CreateUserAsync(dto);
            return result.ToCreatedResult($"/api/users/{result.Data?.Id}");
        }
    }
}
