using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Data;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Repositories.Implementations
{
    public class TaskHistoryRepository : ITaskHistoryRepository
    {
        private readonly AppDbContext _context;

        public TaskHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TaskHistory history)
        {
            await _context.TaskHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(
            Guid taskId,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.TaskHistories.Where(h => h.TaskId == taskId).AsQueryable();

            return await _context.TaskHistories
                .Where(h => h.TaskId == taskId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }
    }
}
