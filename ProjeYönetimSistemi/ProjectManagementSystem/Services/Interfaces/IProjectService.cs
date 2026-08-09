using ProjectManagementSystem.DTOs.Projects;
using ProjectManagementSystem.Enums;

namespace ProjectManagementSystem.Services.Interfaces
{
    public interface IProjectService
    {
        Task CreateAsync(CreateProjectRequestDto request, Guid ownerId, string userRole);
        Task UpdateAsync(Guid projectId, UpdateProjectRequestDto request, Guid userId);

        Task<List<ProjectListDto>> GetAllAsync(
            ProjectStatus? status,
            Guid? ownerId,
            Guid UserId,
            bool isAdmin,
            int page,
            int pageSize);
        Task<List<ProjectMemberListDto>> GetProjectMemberAsync(Guid projectId, Guid userId);
        Task<ProjectDetailDto> GetByIdAsync(Guid projectId, Guid userId);
        Task ArchiveAsync(Guid projectId, Guid userId);
        Task AddMemberAsync(Guid projectId, AddProjectMemberRequestDto request, Guid userId);
        Task RemoveMemberAsync(Guid projectId,Guid memberId, Guid userId);

    }
}
