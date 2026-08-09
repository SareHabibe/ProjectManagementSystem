using ProjectManagementSystem.DTOs.Tasks;
using ProjectManagementSystem.Enums;

namespace ProjectManagementSystem.Services.Interfaces
{
    public interface ITaskService
    {
        Task<TaskDetailDto> CreateAsync(
            CreateTaskRequestDto request,
            Guid createdByUserId);
        Task<TaskDetailDto> GetByIdAsync(Guid id, Guid currentUserId);
        Task UpdateAsync(Guid taskId, UpdateTaskRequestDto request, Guid userId);
        Task UpdateStatusAsync(Guid taskId, TaskItemStatus newStatus, Guid userId);

        Task<List<TaskListDto>> GetAllAsync(
            Guid? projectId,
            TaskItemStatus? status,
            Guid? assignedToUserId,
            TaskPriority? priority,
            DateTime? dueBefore,
            DateTime? dueAfter,
            string? sortBy,
            string? sortDirection,
            int page,
            int pageSize,
            Guid currentUserId);

        Task ArchiveAsync(Guid taskId, Guid userId);
    }
}

