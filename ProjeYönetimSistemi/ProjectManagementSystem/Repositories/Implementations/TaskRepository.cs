using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Data;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories.Interfaces;
using System.Collections.Generic;

namespace ProjectManagementSystem.Repositories.Implementations
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;
        
        public TaskRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task AddAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
        }

        public async Task<TaskItem?> GetByIdAsync(Guid id)
        {
            return await _context.Tasks
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public Task UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            return Task.CompletedTask;
        }

        public async Task AddHistoryAsync(TaskHistory history)
        {
            await _context.TaskHistories.AddAsync(history);
        }

        public async Task<List<TaskItem>> GetAllAsync(
            Guid? projectId, 
            ProjectManagementSystem.Enums.TaskItemStatus? status, 
            Guid? assignedToUserId,
            ProjectManagementSystem.Enums.TaskPriority? priority,
            DateTime? dueBefore,
            DateTime? dueAfter,
            string? sortBy,
            string? sortDirection,
            int page,
            int pageSize)

        {
            var query = _context.Tasks.Where(x => !x.IsDeleted);

            if (projectId.HasValue)
            {
                query = query.Where(x => x.ProjectId == projectId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (assignedToUserId.HasValue)
            {
                query = query.Where(x => x.AssignedToUserId == assignedToUserId.Value);
            }

            if (priority.HasValue)
            {
                query = query.Where(x => x.Priority == priority.Value);
            }

            if (dueBefore.HasValue)
            {
                query = query.Where(x => x.DueDate <= dueBefore.Value);
            }

            if (dueAfter.HasValue)
            {
                query = query.Where(x => x.DueDate >= dueAfter.Value);
            }

            bool isDesc = sortDirection?.ToLower() == "desc";

            if (!string.IsNullOrEmpty(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "duedate" => isDesc ? query.OrderByDescending(x => x.DueDate) : query.OrderBy(x =>x.DueDate),
                    "priority" => isDesc ? query.OrderByDescending(x => x.Priority) : query.OrderBy(x => x.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(x => x.CreatedAt);
            }
                return await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
        }



        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
