using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.DTOs.Auth;
using ProjectManagementSystem.Services.Interfaces;

namespace ProjectManagementSystem.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class AuthController : ControllerBase
        {
            private readonly IAuthService _authService;

            public AuthController(IAuthService authService)
            {
                _authService = authService;
            }

            [HttpPost("register")]
            public async Task<IActionResult> Register(RegisterRequestDto request)
            {
                var result = await _authService.RegisterAsync(request);

                return Ok(result);
            }

            [HttpPost("login")]
            public async Task<IActionResult> Login(LoginRequestDto request)
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }

            
        }
    }
