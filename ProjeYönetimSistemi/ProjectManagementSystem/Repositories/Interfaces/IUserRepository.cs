using ProjectManagementSystem.Models;

namespace ProjectManagementSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task<List<User>> GetAllAsync(
            string? FirstName,
            string? LastName,
            string? Role,
            bool? isActive,
            int page, 
            int pageSize);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}
