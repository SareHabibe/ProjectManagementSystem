using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.DTOs.Tasks;
using ProjectManagementSystem.Enums;
using ProjectManagementSystem.Services.Interfaces;
using System.Security.Claims;

namespace ProjectManagementSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequestDto request)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdString, out Guid createdByUserId))
                {
                    return Unauthorized("Kullanıcı kimliği doğrulanamadı..");
                }

                var result = await _taskService.CreateAsync(request, createdByUserId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? projectId,
            [FromQuery] TaskItemStatus? status,
            [FromQuery] Guid? assignedToUserId,
            [FromQuery] TaskPriority? priority,
            [FromQuery] DateTime? dueBefore,
            [FromQuery] DateTime? dueAfter,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageSize > 50) pageSize = 50;
            if (page < 1) page = 1;

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out Guid currentUserId))
            {
                return Unauthorized("Kullanıcı kimliği doğrulanamadı.");
            }

            var result = await _taskService.GetAllAsync(
                projectId,
                status,
                assignedToUserId,
                priority,
                dueBefore,
                dueAfter,
                sortBy,
                sortDirection,
                page,
                pageSize,
                currentUserId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out Guid currentUserId))
            {
                return Unauthorized("Kullanıcı kimliği doğrulanamadı.");
            }

            try
            {
                var result = await _taskService.GetByIdAsync(id, currentUserId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequestDto request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Geçersiz kullanıcı oturumu.");
            }

            await _taskService.UpdateAsync(id, request, userId);
            return Ok(new { Message = "Görev başarıyla güncellendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Archive(Guid id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Geçersiz kullanıcı oturumu.");
            }

            await _taskService.ArchiveAsync(id, userId);
            return Ok("Görev başarıyla silindi.");
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] TaskItemStatus newStatus)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Geçersiz kullanıcı oturumu.");
            }

            await _taskService.UpdateStatusAsync(id, newStatus, userId);
            return Ok(new { Message = "Görev durumu başarıyla güncellendi." });
        }
    }
}
