using ProjectManagementSystem.Enums;

namespace ProjectManagementSystem.DTOs.Projects
{
    public class AddProjectMemberRequestDto
    {
        public Guid UserId { get; set; }
        public ProjectMemberRole Role { get; set; }
    }
}
