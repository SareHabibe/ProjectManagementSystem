using ProjectManagementSystem.DTOs.Users;
using ProjectManagementSystem.Enums;
using ProjectManagementSystem.Repositories.Interfaces;
using ProjectManagementSystem.Services.Interfaces;

namespace ProjectManagementSystem.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ToggleUserStatusAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            user.IsActive = !user.IsActive;

            await _userRepository.SaveChangesAsync();
        }

        public async Task<List<UserListDto>> GetAllUsersAsync(
            string? FirstName,
            string? LastName,
            string? Role,
            bool? IsActive,
            int page = 1,
            int pageSize = 10)
        {
            var users = await _userRepository.GetAllAsync(
                FirstName,
                LastName,
                Role,
                IsActive,
                page, 
                pageSize);

            return users.Select(x => new UserListDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Role = x.Role.ToString(),
                IsActive = x.IsActive,
                Department = x.Department
            }).ToList();
        }
    }
}