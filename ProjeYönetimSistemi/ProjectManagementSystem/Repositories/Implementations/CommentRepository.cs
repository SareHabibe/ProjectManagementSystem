using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Data;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories.Interfaces;

namespace ProjectManagementSystem.Repositories.Implementations
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {  
            _context = context;
        }

        public async Task AddAsync(Comment comment)
            => await _context.Comments.AddAsync(comment);
       
        public async Task<Comment?> GetByIdAsync(Guid id)
           => await _context.Comments.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        public async Task<List<Comment>> GetAllByTaskIdAsync(
            Guid taskId,
            int page = 1,
            int pageSize = 10)

            => await _context.Comments
            .Include(x => x.User)
            .Where(x => x.TaskId == taskId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        public Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            return Task.CompletedTask; 
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();

    }
}
