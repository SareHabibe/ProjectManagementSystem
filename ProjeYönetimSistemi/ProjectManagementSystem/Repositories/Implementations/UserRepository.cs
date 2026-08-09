using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Data;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories.Interfaces;

namespace ProjectManagementSystem.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
            {
                _context = context;
            }
        
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<List<User>> GetAllAsync(
            string? FirstName,
            string? LastName,
            string? Role,
            bool? IsActive,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.Users
                .Where(x => !x.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(FirstName))
            {
                query = query.Where(x =>x.FirstName.ToLower().Contains(FirstName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(LastName)) 
            {
                query = query.Where(x =>x.LastName.ToLower().Contains(LastName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(Role))
            {
                if (Enum.TryParse<ProjectManagementSystem.Enums.UserRole>(Role, true, out var userRole))
                query = query.Where(x => x.Role == userRole);
            }

            if (IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == IsActive.Value);
            }

            return await query
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
