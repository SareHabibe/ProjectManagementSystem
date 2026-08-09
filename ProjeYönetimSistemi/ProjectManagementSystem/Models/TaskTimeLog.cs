using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models
{
    public class TaskTimeLog
    {
        public Guid Id { get; set; }
        
        public Guid TaskId { get; set; }
        public TaskItem Task { get; set; }
      
        public Guid UserId { get; set; }
        public User User { get; set; }
       
        [Required(ErrorMessage = "Çalışma süresi girilmesi zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Süre 0'dan büyük olmalıdır.")]
        public decimal Hours { get; set; }
       
        [MaxLength(500)]
        public string? Description { get; set; }
        public DateTime WorkDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
