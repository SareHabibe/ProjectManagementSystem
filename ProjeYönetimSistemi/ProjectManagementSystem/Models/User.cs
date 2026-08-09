using ProjectManagementSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models
{
    public class User
    {
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public UserRole Role { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
