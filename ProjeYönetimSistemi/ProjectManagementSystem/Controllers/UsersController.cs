using ProjectManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ProjectManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
       private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
           _userService = userService;
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            if (!User.IsInRole("Admin"))
            {
                return StatusCode(403, new { Message = "Bu işleme sadece Admin yetkisine sahip kişiler erişebilir." });
            }

            await _userService.ToggleUserStatusAsync(id);

            return Ok(new
            {
                Message = "Kullanıcı durumu güncellendi."
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery] string? role,
            [FromQuery] bool? isActive,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)

        {
            if (!User.IsInRole("Admin"))
            {
                return StatusCode(403, new { Message = "Üye listesini görüntüleme yetkiniz yok. Sadece Admin yetkisine sahip kişiler erişebilir." });
            }

            if (pageSize > 50) pageSize = 50;
            if (page < 1) page = 1;

            var users = await _userService.GetAllUsersAsync(
                firstName,
                lastName,
                role,
                isActive,
                page,
                pageSize);
            return Ok(users);
        }
    }
}
