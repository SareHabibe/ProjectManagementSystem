using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.DTOs.Comments;
using ProjectManagementSystem.Services.Interfaces;
using System.Security.Claims;

namespace ProjectManagementSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }


        [HttpPost("tasks/{taskId}/comments")]
        public async Task<IActionResult> Create(Guid taskId, [FromBody] CreateCommentRequestDto request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Kullanıcı kimliği doğrulanamadı.");
            }

            try
            {
                var createdComment = await _commentService.CreateAsync(taskId, request, userId);
                return Ok(createdComment);
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

        [HttpGet("tasks/{taskId}/comments")]
        public async Task<IActionResult> GetByTaskId(
            Guid taskId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });
            } 

            if (pageSize > 50) pageSize = 50;
            if (page < 1) page = 1;

            try
            {
                var result = await _commentService.GetByTaskIdAsync(taskId, userId, page, pageSize);
                return Ok(result);
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

        [HttpPut("comments/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommentRequestDto request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { message = "Geçersiz kullanıcı oturumu." });
            }
            try
            {
                await _commentService.UpdateAsync(id,request, userId, userRole);
                return Ok(new { message = "Yorum başarıyla güncellendi." });
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

        [HttpDelete("comments/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized("Geçersiz kullanıcı oturumu.");
            }

            try
            {
                await _commentService.DeleteAsync(id, userId, userRole);
                return Ok(new { message = "Yorum başarıyla silindi." });
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
