using Microsoft.AspNetCore.Mvc;
using UserService.Dtos;
using UserService.Services;
using Waggle.Common.Models;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service) => _service = service;

        [HttpGet]
        [ProducesResponseType(typeof(GetUsersResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _service.GetAllUsers();
            return result.ToActionResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetUserByIdResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _service.GetUserById(id);
            return result.ToActionResult();
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateUserResponseDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUser(UserCreateDto dto)
        {
            var result = await _service.CreateUser(dto);
            return result.ToCreatedResult($"/api/users/{result.Data?.Id}");
        }
    }
}
