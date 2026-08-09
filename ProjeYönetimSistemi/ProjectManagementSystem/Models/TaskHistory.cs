using ProjectManagementSystem.Enums;

namespace ProjectManagementSystem.Models
{
    public class TaskHistory
    {
        public Guid Id { get; set; }

       
        public Guid TaskId { get; set; }
        public TaskItem Task { get; set; }

        public Guid ChangedByUserId { get; set; }
        public User ChangedByUser { get; set; }
       
        public ChangeType ChangeType { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
