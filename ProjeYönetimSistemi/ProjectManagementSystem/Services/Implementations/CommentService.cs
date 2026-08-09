using ProjectManagementSystem.DTOs.Comments;
using ProjectManagementSystem.Enums;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories.Implementations;
using ProjectManagementSystem.Repositories.Interfaces;
using ProjectManagementSystem.Services.Interfaces;

namespace ProjectManagementSystem.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;

        public CommentService(
            ICommentRepository commentRepository,
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        public async Task<CommentListDto> CreateAsync(Guid taskId, CreateCommentRequestDto request, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                throw new Exception("Görev bulunamadı.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            if (user.Role == UserRole.Viewer)
            {
                throw new UnauthorizedAccessException("Viewer rolündeki kullanıcılar göreve yorum yapamaz.");
            }

            var project = await _projectRepository.GetByIdAsync(task.ProjectId);
            if (project == null)
            {
                throw new Exception("Proje bulunamadı.");
            }

            var member = await _projectRepository.GetMemberAsync(task.ProjectId, userId);
            if (member != null && member.Role == ProjectMemberRole.Viewer)
            {
                throw new UnauthorizedAccessException("Proje bazlı Viewer rolündeki kullanıcılar yorum yapamaz.");
            }

            bool isAdmin = user.Role == UserRole.Admin;
            bool isProjectOwner = project.OwnerId == userId;
            bool isTaskCreator = task.CreatedByUserId == userId;
            string roleName = user.Role.ToString();

            bool hasAccess = false;

            if (isAdmin)
            {
                hasAccess = true;
            }

            else if (roleName == "ProjectManager" && (isProjectOwner || isTaskCreator))
            {
                hasAccess = true;
            }

            else
            {

                if (member == null && !isProjectOwner)
                {
                    throw new Exception("Bu göreve yorum yapabilmek için ilgili projenin üyesi olmalısınız.");
                }

                if (task.AssignedToUserId == userId)
                {
                    hasAccess = true;
                }
                else
                {
                    throw new UnauthorizedAccessException("Yalnızca kendi üzerinize atanan görevlere yorum yapabilirsiniz.");
                }
            }

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("Bu göreve yorum yapma yetkiniz bulunmamaktadır.");
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = request.Content,
                TaskId = taskId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();

            return new CommentListDto
            {
                Id = comment.Id,
                Content = comment.Content,
                TaskId = comment.TaskId,
                UserId = comment.UserId,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<List<CommentListDto>> GetByTaskIdAsync(
            Guid taskId,
            Guid userId,
            int page = 1,
            int pageSize = 10)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                throw new Exception("Görev bulunamadı.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            var project = await _projectRepository.GetByIdAsync(task.ProjectId);
            if (project == null)
            {
                throw new Exception("Proje bulunamadı.");
            }

            bool isAdmin = user.Role == UserRole.Admin;
            bool isProjectOwner = project.OwnerId == userId;
            bool isTaskCreator = task.CreatedByUserId == userId;
            string roleName = user.Role.ToString();

            bool hasAccess = false;

            if (isAdmin)
            {
                hasAccess = true;
            }
            else if (roleName == "ProjectManager" && (isProjectOwner || isTaskCreator))
            {
                hasAccess = true;
            }
            else
            {
                var member = await _projectRepository.GetMemberAsync(task.ProjectId, userId);
                if (member == null && !isProjectOwner)
                {
                    throw new Exception("Bu görevin yorumlarını görebilmek için ilgili projenin üyesi olmalısınız.");
                }

                if (user.Role == UserRole.Viewer || member?.Role == ProjectMemberRole.Viewer)
                {
                    hasAccess = true;
                }

                else if (task.AssignedToUserId == userId)
                {
                    hasAccess = true;
                }

                else
                {
                    throw new UnauthorizedAccessException("Team Member rolündeki kullanıcılar yalnızca kendi üzerlerine atanan görevlerin yorumlarını görüntüleyebilir.");
                }
            }

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("Bu görevin yorumlarını görüntüleme yetkiniz bulunmamaktadır.");
            }

            var comments = await _commentRepository.GetAllByTaskIdAsync(taskId);
            return comments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CommentListDto
            {
                Id = c.Id,
                Content = c.Content,
                TaskId = taskId,
                UserId = c.UserId,
                UserName = c.User.FirstName + " " + c.User.LastName,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList();
        }

        public async Task UpdateAsync(Guid commentId, UpdateCommentRequestDto request, Guid userId, string userRole)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
            {
                throw new Exception("Yorum bulunamadı.");
            }

            var task = await _taskRepository.GetByIdAsync(comment.TaskId);
            if (task == null)
            {
                throw new Exception("Görev bulunamadı.");
            }

            var project = await _projectRepository.GetByIdAsync(task.ProjectId);
            if (project == null)
            {
                throw new Exception("Proje bulunamadı.");
            }

            bool isAdmin = userRole == "Admin";
            bool isProjectOwner = project.OwnerId == userId;
            bool isCommentOwner = comment.UserId == userId;
            bool isViewer = userRole == "Viewer";

            if (isViewer)
            {
                throw new UnauthorizedAccessException("Viewer rolündeki kullanıcılar yorum güncelleyemez.");
            }

            if (!comment.UserId.Equals(userId) && !isProjectOwner && !isAdmin)
            {
                throw new UnauthorizedAccessException("Bu yorumu güncelleme yetkiniz yok. Sadece yorum sahibi, Proje Yöneticisi veya Admin güncelleyebilir.");
            }

            comment.Content = request.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            await _commentRepository.UpdateAsync(comment);
            await _commentRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid commentId, Guid userId, string userRole)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
            {
                throw new Exception("Yorum bulunamadı.");
            }

            var task = await _taskRepository.GetByIdAsync(comment.TaskId);
            if (task == null)
            {
                throw new Exception("Görev bulunamadı.");
            }

            var project = await _projectRepository.GetByIdAsync(task.ProjectId);
            if (project == null)
            {
                throw new Exception("Proje bulunamadı.");
            }

            if (userRole == "Viewer")
            {
                throw new UnauthorizedAccessException("Viewer rolündeki kullanıcıların yorum silme yetkisi bulunmamaktadır.");
            }

            bool isAdmin = userRole == "Admin";
            bool isProjectOwner = project.OwnerId == userId;
            bool isCommentOwner = comment.UserId == userId;
            bool isProjectManager = userRole == "ProjectManager";
            bool isTeamMember = userRole == "TeamMember";

            bool hasAccess = false;

            if (isAdmin)
            {
                hasAccess = true;
            }
            else if (isProjectManager && (isProjectOwner || isCommentOwner))
            {
                hasAccess = true;
            }
            else if (isTeamMember && isCommentOwner)
            {
                hasAccess = true;
            }

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("Bu yorumu silme yetkiniz bulunmamaktadır.");
            }

            comment.IsDeleted = true;
            comment.UpdatedAt = DateTime.UtcNow;

            await _commentRepository.UpdateAsync(comment);
            await _commentRepository.SaveChangesAsync();
        }
    }
}
