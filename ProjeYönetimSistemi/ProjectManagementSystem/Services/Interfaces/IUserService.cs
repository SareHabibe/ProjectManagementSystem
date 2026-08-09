using ProjectManagementSystem.DTOs.Users;
using ProjectManagementSystem.Enums;
using ProjectManagementSystem.Models;

namespace ProjectManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        Task ToggleUserStatusAsync(Guid userId);
        Task<List<UserListDto>> GetAllUsersAsync(
            string? FirstName,
            string? LastName,
            string? Role,
            bool? IsActive,
            int page, 
            int pageSize);
    }
}