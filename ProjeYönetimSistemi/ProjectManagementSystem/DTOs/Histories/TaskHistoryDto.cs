using System;

namespace ProjectManagementSystem.DTOs.Histories
{
    public class TaskHistoryDto
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public Guid ChangedByUserId { get; set; }
        public string ChangeType { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? Description  { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
