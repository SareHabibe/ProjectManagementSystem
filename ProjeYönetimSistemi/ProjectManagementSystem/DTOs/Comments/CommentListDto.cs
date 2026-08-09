namespace ProjectManagementSystem.DTOs.Comments
{
    public class CommentListDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
