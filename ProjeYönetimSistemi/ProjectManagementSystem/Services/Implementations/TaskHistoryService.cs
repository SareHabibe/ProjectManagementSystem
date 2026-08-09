using ProjectManagementSystem.DTOs.Histories;
using ProjectManagementSystem.Enums;
using ProjectManagementSystem.Repositories.Interfaces;
using ProjectManagementSystem.Services.Interfaces;

namespace ProjectManagementSystem.Services.Implementations
{
    public class TaskHistoryService : ITaskHistoryService
    {
        private readonly ITaskHistoryRepository _historyRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;

        public TaskHistoryService(ITaskHistoryRepository historyRepository, ITaskRepository taskRepository, IProjectRepository projectRepository)
        {
            _historyRepository = historyRepository;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<TaskHistoryDto>> GetHistoriesByTaskIdAsync(
            Guid taskId,
            Guid userId,
            string userRole,
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

            bool isAdmin = !string.IsNullOrEmpty(userRole) &&
                           userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);

            if (!isAdmin)
            {
                bool isProjectOwner = project.OwnerId == userId;
                var member = await _projectRepository.GetMemberAsync(task.ProjectId, userId);

                if (member == null && !isProjectOwner)
                {
                    throw new UnauthorizedAccessException("Bu projenin görev geçmişini görüntüleme yetkiniz yok.");
                }

                bool isTeamMember = userRole.Equals("TeamMember", StringComparison.OrdinalIgnoreCase);
                bool isViewer = (member != null && member.Role == ProjectMemberRole.Viewer) ||
                                userRole.Equals("Viewer", StringComparison.OrdinalIgnoreCase);

                if (isTeamMember || isViewer)
                {
                    if (task.AssignedToUserId != userId)
                    {
                        throw new UnauthorizedAccessException("Sadece size atanmış görevlerin geçmişini görüntüleyebilirsiniz.");
                    }
                }
            }
            var histories = await _historyRepository.GetByTaskIdAsync(taskId);

            return histories.Select(h => new TaskHistoryDto
            {
                Id = h.Id,
                TaskId = h.TaskId,
                ChangedByUserId = h.ChangedByUserId,
                ChangeType = h.ChangeType switch
                {
                    ChangeType.Updated => "Görev Güncellendi",
                    ChangeType.StatusChanged => "Durum Değiştirildi",
                    ChangeType.PriorityChanged => "Öncelik Değiştirildi",
                    ChangeType.AssignedUserChanged => "Atanan Kişi Değiştirildi",
                    _ => h.ChangeType.ToString()
                },
                OldValue = h.OldValue,
                NewValue = h.NewValue,
                Description = h.Description,
                CreatedAt = h.CreatedAt,
            });
        }

    }
}
