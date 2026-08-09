using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Services.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize]
    public class HistoriesController : ControllerBase
    {
        private readonly ITaskHistoryService _historyService;

        public HistoriesController(ITaskHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet("{taskId}/histories")]
        public async Task<IActionResult> GetTaskHistories(
            Guid taskId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });
            }

            if (pageSize > 50) pageSize = 50;
            if (page < 1) page = 1;

            try
            {
                var histories = await _historyService.GetHistoriesByTaskIdAsync(
                     taskId,
                     userId,
                     userRole,
                     page,
                     pageSize);

                return Ok(histories);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

