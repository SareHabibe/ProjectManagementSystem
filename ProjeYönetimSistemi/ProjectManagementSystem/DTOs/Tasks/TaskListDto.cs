using ProjectManagementSystem.Enums;

namespace ProjectManagementSystem.DTOs.Tasks
{
    public class TaskListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public Guid? AssignedToUserId { get; set; }

        public TaskItemStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
