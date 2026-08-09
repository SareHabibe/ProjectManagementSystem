using ProjectManagementSystem.Data; 
using ProjectManagementSystem.Models; 
using Microsoft.EntityFrameworkCore;


using ProjectManagementSystem.Repositories.Interfaces;

namespace ProjectManagementSystem.Repositories.Implementations
{
    public class TaskTimeLogRepository : ITaskTimeLogRepository
    {
        private readonly AppDbContext _context;

        public TaskTimeLogRepository(AppDbContext context )
        {
            _context = context;
        }

        public async Task AddAsync(TaskTimeLog timeLog)
        {
            await _context.TaskTimeLogs.AddAsync(timeLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskTimeLog>> GetByTaskIdAsync(
            Guid taskId, 
            Guid? userId = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.TaskTimeLogs.Where(t => t.TaskId == taskId).AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(t => t.UserId == userId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(t => t.WorkDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.WorkDate <= endDate.Value);
            }
            return await query
                .OrderByDescending(t => t.WorkDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalHoursByTaskIdAsync(Guid taskId)
        {
            return await _context.TaskTimeLogs
                .Where(t => t.TaskId == taskId)
                .SumAsync(t => t.Hours);
        }
    }
}
