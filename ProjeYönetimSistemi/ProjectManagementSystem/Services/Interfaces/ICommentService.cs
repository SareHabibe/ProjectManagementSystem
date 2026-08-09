using ProjectManagementSystem.DTOs.Comments;

namespace ProjectManagementSystem.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentListDto> CreateAsync(
            Guid taskId, 
            CreateCommentRequestDto request, 
            Guid userId);
        
        Task<List<CommentListDto>> GetByTaskIdAsync(
            Guid taskId,
            Guid userId,
            int page = 1,
            int pageSize = 10);
        
        Task UpdateAsync(
            Guid commentId, 
            UpdateCommentRequestDto request, 
            Guid userId, 
            string userRole);
     
        Task DeleteAsync(
            Guid commentId,
            Guid userId, 
            string userRole);
    }
}
