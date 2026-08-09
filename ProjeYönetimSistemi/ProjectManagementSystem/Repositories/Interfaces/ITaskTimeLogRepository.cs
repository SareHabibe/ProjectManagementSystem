using ProjectManagementSystem.Models;

namespace ProjectManagementSystem.Repositories.Interfaces
{
    public interface ITaskTimeLogRepository
    {
        Task AddAsync(TaskTimeLog timeLog);
        Task<IEnumerable<TaskTimeLog>> GetByTaskIdAsync(
            Guid taskId, 
            Guid? userId = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            int page = 1, 
            int pageSize = 10);
        Task<decimal> GetTotalHoursByTaskIdAsync(Guid taskId);
    }
}
