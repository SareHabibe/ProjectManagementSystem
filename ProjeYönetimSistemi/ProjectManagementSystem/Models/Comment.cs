using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models
{
    public class Comment
    {
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; }

       
        public Guid TaskId { get; set; }

        public TaskItem Task { get; set; }


        public Guid UserId { get; set; }

        public User User { get; set; }

        
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
