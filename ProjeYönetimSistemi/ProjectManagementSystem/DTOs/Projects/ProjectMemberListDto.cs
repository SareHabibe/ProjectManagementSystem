using ProjectManagementSystem.Enums;

namespace ProjectManagementSystem.DTOs.Projects
{
    public class ProjectMemberListDto
    {
        public Guid UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public ProjectManagementSystem.Enums.ProjectMemberRole Role { get; set; }
    }
}
