using ProjectManagementSystem.DTOs.Histories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Services.Interfaces
{
    public interface ITaskHistoryService
    {
        Task<IEnumerable<TaskHistoryDto>> GetHistoriesByTaskIdAsync(
            Guid taskId,
            Guid userId,
            string userRole,
            int page = 1,
            int pageSize = 10);
    }
}
