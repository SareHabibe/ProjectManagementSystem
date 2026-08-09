using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.DTOs.Comments
{
    public class CreateCommentRequestDto
    {
        [Required(ErrorMessage = "Yorum içeriği boş bırakılamaz.")]
        public string Content { get; set; } = string.Empty;
    }
}
