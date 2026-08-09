using ProjectManagementSystem.Enums;
using System.ComponentModel.DataAnnotations;
namespace ProjectManagementSystem.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }

        
        public Guid ProjectId { get; set; }

        public Project Project { get; set; }

       
        public Guid? AssignedToUserId { get; set; }

        public User? AssignedToUser { get; set; }

       
        public Guid CreatedByUserId { get; set; }

        public User CreatedByUser { get; set; }

        
        public ProjectManagementSystem.Enums.TaskItemStatus Status { get; set; }

        public TaskPriority Priority { get; set; }

        public DateTime? DueDate { get; set; }

        public decimal? EstimatedHours { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
