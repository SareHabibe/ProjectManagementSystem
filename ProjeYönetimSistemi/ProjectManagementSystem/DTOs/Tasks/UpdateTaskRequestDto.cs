using ProjectManagementSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.DTOs.Tasks
{
    public class UpdateTaskRequestDto
    {
        [Required(ErrorMessage = "Görev başlığı zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        public string Title { get; set; }

        [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; }

        public TaskItemStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid? AssignedToUserId { get; set; }
    }
}
