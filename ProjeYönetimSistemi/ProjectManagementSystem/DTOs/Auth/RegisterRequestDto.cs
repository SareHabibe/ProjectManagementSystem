using ProjectManagementSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.DTOs.Auth
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "İsim alanı zorunludur.")]
        [MaxLength(50, ErrorMessage = "İsim en fazla 50 karakter olabilir.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyisim alanı zorunludur.")]
        [MaxLength(50, ErrorMessage = "Soyisim en fazla 50 karakter olabilir.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        [MaxLength(100, ErrorMessage = "Departman alanı en fazla 100 karakter olabilir.")]
        public string? Department { get; set; }
        
    }
}

