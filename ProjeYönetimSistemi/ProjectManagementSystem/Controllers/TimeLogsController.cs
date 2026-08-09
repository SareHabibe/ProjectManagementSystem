using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.DTOs.TimeLogs;
using ProjectManagementSystem.Enums;
using ProjectManagementSystem.Services.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize]
    public class TimeLogsController : ControllerBase
    {
        private readonly ITaskTimeLogService _timeLogService;

        public TimeLogsController(ITaskTimeLogService timeLogService)
        {
            _timeLogService = timeLogService;
        }

       
        [HttpPost("{taskId}/time-logs")]
        public async Task<IActionResult> AddTimeLog(Guid taskId, [FromBody] CreateTaskTimeLogRequestDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });
            }

            try
            {
                await _timeLogService.AddTimeLogAsync(taskId, userId, userRole, dto);
                return Ok(new { message = "Zaman kaydı başarıyla eklendi." });
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


        [HttpGet("{taskId}/time-logs")]
        public async Task<ActionResult> GetTimeLogs(
            Guid taskId,
            [FromQuery] Guid? userId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var currentUserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!Guid.TryParse(currentUserIdString, out Guid currentUserId) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });
            }

            if (pageSize > 50) pageSize = 50;
            if (page < 1) page = 1;
            
            try
            {
                var logs = await _timeLogService.GetLogsByTaskIdAsync(
                    taskId,
                    currentUserId,
                    userRole,
                    userId,
                    startDate,
                    endDate,
                    page,
                    pageSize);

                return Ok(logs);
        }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
