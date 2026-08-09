using ProjectManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Repositories.Interfaces
{
    public interface ITaskHistoryRepository
    {
        Task AddAsync(TaskHistory history);
        Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(
            Guid taskId,
            int page = 1,
            int pageSize = 10);
    }
}
