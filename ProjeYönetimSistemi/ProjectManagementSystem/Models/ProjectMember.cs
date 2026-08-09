using ProjectManagementSystem.Enums;

namespace ProjectManagementSystem.Models
{
    public class ProjectMember
    {
        public Guid Id { get; set; }

        
        public Guid ProjectId { get; set; }

        public Project Project { get; set; }

       
        public Guid UserId { get; set; }

        public User User { get; set; }

       
        public ProjectMemberRole Role { get; set; }

        public DateTime JoinedAt { get; set; }
       
        public bool IsActive { get; set; }
    }
}
