using ProjectManagementSystem.Enums;

namespace ProjectManagementSystem.DTOs.Projects
{
    public class ProjectDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ProjectStatus Status { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedAt { get; set; }  

    }
}
