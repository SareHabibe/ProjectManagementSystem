using ProjectManagementSystem.Models;
using ProjectManagementSystem.DTOs.TimeLogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Services.Interfaces
{
    public interface ITaskTimeLogService
    {
        Task AddTimeLogAsync(Guid taskId, Guid userId, string userRole, CreateTaskTimeLogRequestDto dto);
        Task<IEnumerable<TaskTimeLogResponseDto>> GetLogsByTaskIdAsync(
            Guid taskId,
            Guid userId,
            string userRole,
            Guid? filterUserId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int page = 1,
            int pageSize = 10);
    }

}
