using ProjectManagementSystem.DTOs.Tasks;
using ProjectManagementSystem.Models;

namespace ProjectManagementSystem.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskItem task);
        Task<TaskItem?> GetByIdAsync(Guid İd);
        Task UpdateAsync(TaskItem task);
        Task AddHistoryAsync(TaskHistory history);
        Task SaveChangesAsync();

        Task<List<TaskItem>> GetAllAsync(
            Guid? projectId, 
            ProjectManagementSystem.Enums.TaskItemStatus? status, 
            Guid? assignedToUserId,
            ProjectManagementSystem.Enums.TaskPriority? priority,
            DateTime? dueBefore,
            DateTime? dueAfter,
            string? sortBy,
            string? sortDirection,
            int page, 
            int pageSize);
    }
}
