using AuthService.Dtos;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;
using Waggle.Common.Models;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IKeycloakService _keycloakService;

        public AuthController(IKeycloakService keycloakService)
        {
            _keycloakService = keycloakService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _keycloakService.PasswordGrantAsync(request.Username, request.Password);
            return result.ToActionResult();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _keycloakService.CreateUserAsync(request.Username, request.Email, request.Password);
            return result.ToCreatedResult();
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _keycloakService.RefreshTokenAsync(request.RefreshToken);
            return result.ToActionResult();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
        {
            var result = await _keycloakService.LogoutAsync(request.RefreshToken);
            return result.ToActionResult();
        }

        [HttpGet("validate")]
        public async Task<IActionResult> Validate()
        {
            var authHeader = Request.Headers.Authorization.ToString();
            var result = await _keycloakService.ValidateAsync(authHeader);
            return result.ToActionResult();
        }
    }
}