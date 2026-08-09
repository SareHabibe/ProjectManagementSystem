using ProjectManagementSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.DTOs.Projects
{
    public class CreateProjectRequestDto
    {
        [Required(ErrorMessage = "Proje adı zorunludur.")]
        [MaxLength(200, ErrorMessage = "Name alanı en fazla 200 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty ;

        [Required(ErrorMessage = "Başlangıç tarihi zorunludur.")]
        public DateTime StartDate { get; set; } 
        public DateTime? EndDate { get; set; }

        public ProjectStatus Status { get; set; }
    }
}
