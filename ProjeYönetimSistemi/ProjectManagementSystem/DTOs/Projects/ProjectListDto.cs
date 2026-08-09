using ProjectManagementSystem.Enums;

namespace ProjectManagementSystem.DTOs.Projects
{
    public class ProjectListDto
    {
        public Guid Id { get; set; }    
        public string Name { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid OwnerId { get; set; }   
    }
}
