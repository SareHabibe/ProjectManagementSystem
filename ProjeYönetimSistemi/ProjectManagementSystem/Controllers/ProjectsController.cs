using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Services.Interfaces;
using ProjectManagementSystem.DTOs.Projects;
using ProjectManagementSystem.Enums;
using System.Security.Claims;

namespace ProjectManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectRequestDto request)
        {
            var ownerId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            await _projectService.CreateAsync(request, ownerId, userRole);

            return Ok("Proje başarıyla oluşturuldu.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] ProjectStatus? status,
            [FromQuery] Guid? ownerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageSize > 50) pageSize = 50;
            if (pageSize < 1) pageSize = 1;

            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı." });
                }

                Guid currentUserId = Guid.Parse(userIdClaim);
                bool isAdmin = User.IsInRole("Admin");

                var projects = await _projectService.GetAllAsync
                    (status, ownerId,currentUserId,isAdmin, page, pageSize);

                return Ok(projects);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı." });
                }

                Guid userId = Guid.Parse(userIdString);

                var project = await _projectService.GetByIdAsync(id, userId);

                if (project == null)
                {
                    return NotFound(new { Message = "Proje bulunamadı." });
                }

                return Ok(project);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new {Message = ex.Message});
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            Guid id,
            UpdateProjectRequestDto request)
        {
            try
            {
                var userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı." });
                }

                Guid userId = Guid.Parse(userIdString);

                await _projectService.UpdateAsync(id, request, userId);
                return Ok("Proje başarıyla güncellendi.");
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Archive(Guid id)
        {
            try
            {
                var userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı." });
                }

                Guid userId = Guid.Parse(userIdString);

                await _projectService.ArchiveAsync(id, userId);

                return Ok("Proje başarıyla silindi.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message});
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message});
            }
        }

        [HttpPost("{projectId}/members")]
        public async Task<IActionResult> AddMember(
            Guid projectId,
            AddProjectMemberRequestDto request)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı." });
                }
                Guid userId = Guid.Parse(userIdString);

                await _projectService.AddMemberAsync(projectId, request, userId);
                return Ok("Üye başarıyla projeye eklendi.");
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

        [HttpGet("{projectId}/members")]
        public async Task<IActionResult> GetProjectMember(Guid projectId)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı." });
                }

                Guid userId = Guid.Parse(userIdString);
                var members = await _projectService.GetProjectMemberAsync(projectId, userId);
                return Ok(members);
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

        [HttpDelete("{projectId}/members/{memberId}")]
        public async Task<IActionResult> RemoveMember(Guid projectId,Guid memberId)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı." });
                }

                Guid userId = Guid.Parse(userIdString);

                await _projectService.RemoveMemberAsync( projectId, memberId, userId);

                return Ok("Üye başarıyla projeden çıkarıldı.");
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
    }
}
