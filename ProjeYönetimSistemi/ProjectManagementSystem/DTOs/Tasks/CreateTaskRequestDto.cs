using ProjectManagementSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.DTOs.Tasks
{
    public class CreateTaskRequestDto
    {
        [Required(ErrorMessage =  "Görev başlığı (Title) zorunludur.")]
        [MaxLength(200, ErrorMessage="Görev başlığı en fazla 200 karakter olabilir.")]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
       
        [Required(ErrorMessage ="ProjectId alanı zorunludur.")]
        public Guid ProjectId { get; set; }
        public Guid? AssignedToUserId { get; set; }

        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? EstimatedHours { get; set; }
    }
}
