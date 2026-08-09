using ProjectManagementSystem.Models;
using ProjectManagementSystem.Enums;

namespace ProjectManagementSystem.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        Task AddAsync(Project project);
        Task<Project?> GetByIdAsync(Guid id);
        Task<List<Project>> GetAllAsync(
            ProjectStatus? status,
            Guid? ownerId,
            Guid currentUserId,
            bool isAdmin,
            int page,
            int pageSize);

        Task<List<ProjectMember>> GetProjectMemberProjectIdAsync(Guid projectId);

        Task UpdateAsync(Project project);
        Task AddMemberAsync(ProjectMember projectMember);
        Task<ProjectMember?> GetMemberAsync (Guid projectId, Guid userId);
        Task RemoveMemberAsync(ProjectMember member);
        Task SaveChangesAsync();
    }
}
