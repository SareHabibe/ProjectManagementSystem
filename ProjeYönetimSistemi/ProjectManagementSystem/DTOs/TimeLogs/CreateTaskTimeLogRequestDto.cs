using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.DTOs.TimeLogs
{
    public class CreateTaskTimeLogRequestDto
    {
        [Required(ErrorMessage = "Çalışma süresi girilmesi zorunludur.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Süre 0'dan büyük olmalıdır.")]
        public decimal Hours { get; set; } 

        [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; } 
        public DateTime WorkDate { get; set; }
    }
}
