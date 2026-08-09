using ProjectManagementSystem.DTOs.TimeLogs;
using ProjectManagementSystem.Enums;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories;
using ProjectManagementSystem.Repositories.Interfaces;
using ProjectManagementSystem.Services.Interfaces;

namespace ProjectManagementSystem.Services.Implementations
{
    public class TaskTimeLogService : ITaskTimeLogService
    {
        private readonly ITaskTimeLogRepository _timeLogRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;

        public TaskTimeLogService(
            ITaskTimeLogRepository timeLogRepository,
            ITaskRepository taskRepository,
            IProjectRepository projectRepository)
        {
            _timeLogRepository = timeLogRepository;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        public async Task AddTimeLogAsync(Guid taskId, Guid userId, string userRole, CreateTaskTimeLogRequestDto dto)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);

            if (task == null)
            {
                throw new Exception("Görev bulunamadı.");
            }

            var project = await _projectRepository.GetByIdAsync(task.ProjectId);

            if (project == null)
            {
                throw new Exception("Proje bulunamadı.");
            }

            bool isAdmin = !string.IsNullOrEmpty(userRole) && userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);

            if (!isAdmin)
            {
                bool isProjectOwner = project.OwnerId == userId;
                var member = await _projectRepository.GetMemberAsync(task.ProjectId, userId);

                if (member == null && !isProjectOwner)
                {
                    throw new UnauthorizedAccessException("Bu projeye üye olmadığınız için zaman kaydı giremezsiniz.");
                }

                if (member != null && member.Role == ProjectMemberRole.Viewer)
                {
                    throw new UnauthorizedAccessException("Viewer rolündeki kullanıcılar zaman kaydı giremez.");
                }

                if (userRole == "ProjectManager")
                {
                    if (project.OwnerId != userId)
                    {
                        throw new UnauthorizedAccessException("Sadece sahibi olduğunuz projelerin görevlerine zaman kaydı ekleyebilirsiniz.");
                    }
                }

                if (userRole == "TeamMember")
                {
                    if (task.AssignedToUserId != userId)
                    {
                        throw new UnauthorizedAccessException("Sadece size atanmış görevlere zaman kaydı ekleyebilirsiniz.");
                    }
                }
            }

            if (dto.Hours <= 0)
            {
                throw new Exception("Çalışma süresi 0'dan büyük olmalıdır.");
            }

            var timeLog = new TaskTimeLog
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = userId,
                Hours = dto.Hours,
                Description = dto.Description,
                WorkDate = dto.WorkDate,
                CreatedAt = DateTime.UtcNow
            };

            await _timeLogRepository.AddAsync(timeLog);
        }

        public async Task<IEnumerable<TaskTimeLogResponseDto>> GetLogsByTaskIdAsync(
            Guid taskId,
            Guid userId,
            string userRole,
            Guid? filterUserId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int page = 1,
            int pageSize = 10)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                throw new Exception("Görev bulunamadı.");
            }

            var project = await _projectRepository.GetByIdAsync(task.ProjectId);
            if (project == null)
            {
                throw new Exception("Proje bulunamadı.");
            }

            bool isAdmin = !string.IsNullOrEmpty(userRole) && userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);

            if (!isAdmin)
            {
                bool isProjectOwner = project.OwnerId == userId;
                var member = await _projectRepository.GetMemberAsync(task.ProjectId, userId);

                if (member == null && !isProjectOwner)
                {
                    throw new UnauthorizedAccessException("Bu projenin zaman kayıtlarını görüntüleme yetkiniz yok.");
                }

                if (userRole.Equals("ProjectManager", StringComparison.OrdinalIgnoreCase))
                {
                    if (!isProjectOwner && member == null)
                    {
                        throw new UnauthorizedAccessException("Sadece sahibi veya üyesi olduğunuz projelerin zaman kayıtlarını görebilirsiniz.");
                    }
                }

                bool isTeamMember = userRole.Equals("TeamMember", StringComparison.OrdinalIgnoreCase);
                bool isViewer = (member != null && member.Role == ProjectMemberRole.Viewer) ||
                                userRole.Equals("Viewer", StringComparison.OrdinalIgnoreCase);

                if (isTeamMember || isViewer)
                {
                    if (task.AssignedToUserId != userId)
                    {
                        throw new UnauthorizedAccessException("Sadece size atanmış görevlere ait zaman kayıtlarını görüntüleyebilirsiniz.");
                    }
                }
            }

            var logs = await _timeLogRepository.GetByTaskIdAsync(taskId, filterUserId, startDate, endDate, page, pageSize);
            return logs.Select(x => new TaskTimeLogResponseDto
                {
                    Id = x.Id,
                    TaskId = x.TaskId,
                    UserId = x.UserId,
                    Hours = x.Hours,
                    Description = x.Description,
                    WorkDate = x.WorkDate,
                    CreatedAt = x.CreatedAt
                });
            }
        }
    }
