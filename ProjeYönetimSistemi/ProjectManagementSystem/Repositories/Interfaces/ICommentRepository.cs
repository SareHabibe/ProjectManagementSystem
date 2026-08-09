using ProjectManagementSystem.Models;

namespace ProjectManagementSystem.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task AddAsync(Comment comment);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<List<Comment>> GetAllByTaskIdAsync(
            Guid taskId,
            int page = 1, 
            int pageSize = 10);
        Task UpdateAsync(Comment comment);
        Task SaveChangesAsync();
    }
}
