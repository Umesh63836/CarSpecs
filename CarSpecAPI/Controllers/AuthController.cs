using CarSpecAPI.Data.Models.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CarSpecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var response = await authService.RegisterAsync(dto);

            if (response == null)
                return Unauthorized("Failed to register - Username already exists!");

            return Ok(response);
        }

        [HttpPost("login")]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Login(AdminLoginDto dto)
        {
            var response = await authService.LoginAsync(dto);

            if (response == null)
                return Unauthorized("Invalid username or password.");

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(string refreshToken)
        {
            var response = await authService.RefreshTokenAsync(refreshToken);

            if (response == null)
                return Unauthorized("Invalid refresh token.");

            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string refreshToken)
        {
            await authService.LogoutAsync(refreshToken);

            return Ok();
        }
    }
}
