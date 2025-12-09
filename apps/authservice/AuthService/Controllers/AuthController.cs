using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.Services;
using Waggle.Common.Auth;
using Waggle.Common.Controllers;
using Waggle.Common.Models;
using Waggle.Common.Results.Extensions;

namespace Waggle.AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<TokenResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.PasswordGrantAsync(request);
            return result.ToActionResult();
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.CreateUserAsync(request);
            return result.ToCreatedResult();
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(ApiResponse<TokenResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
        {
            var result = await _authService.LogoutAsync(request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("validate")]
        [ProducesResponseType(typeof(ApiResponse<UserInfoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Validate()
        {
            ValidateTokenRequestDto dto = new() { BearerToken = Request.Headers.Authorization.ToString() };
            var result = await _authService.ValidateAsync(dto);

            if (result.Data != null)
            {
                var user = result.Data;

                Response.Headers.Append("X-User-ID", user.Sub);
                Response.Headers.Append("X-User-Username", user.Username);
                Response.Headers.Append("X-User-Email", user.Email ?? "");
                Response.Headers.Append("X-User-Name", user.Name ?? user.Username);
                Response.Headers.Append("X-User-Roles", string.Join(',', user.Roles ?? []));
            }

            var isGateway = Request.Headers.ContainsKey("X-ForwardAuth");
            return isGateway ? Ok() : result.ToActionResult();
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(DeleteUserRequestDto request)
        {
            var result = await _authService.DeleteUserAsync(request, CurrentUser);
            return result.ToActionResult();
        }
    }
}