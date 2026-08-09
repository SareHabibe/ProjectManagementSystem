using ProjectManagementSystem.DTOs.Tasks;
using ProjectManagementSystem.Enums;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories.Interfaces;
using ProjectManagementSystem.Services.Interfaces;

namespace ProjectManagementSystem.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;

        public TaskService(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        private async Task ValidateUserIsProjectMember(Guid userId, Guid projectId)
        {
            var member = await _projectRepository.GetMemberAsync(projectId, userId);

            if (member == null)
            {
                throw new Exception("İşlem yapmak için ilgili projede ekip üyesi olmalısınız.");
            }

            if (member.Role == ProjectManagementSystem.Enums.ProjectMemberRole.Viewer)
            {
                throw new Exception("Viewer rolündeki kullanıcılar görev, yorum veya zaman kaydı yapamaz.");
            }
        }

        public async Task<TaskDetailDto> CreateAsync(CreateTaskRequestDto request, Guid createdByUserId)
        {
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);

            if (project == null)
            {
                throw new Exception("Proje bulunamadı.");
            }

            var user = await _userRepository.GetByIdAsync(createdByUserId);
            
            if(user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            bool isAdmin = user.Role == ProjectManagementSystem.Enums.UserRole.Admin;
            bool isOwner = project.OwnerId == createdByUserId;
            string roleName = user.Role.ToString();
            bool hasAccess = false;

            if (isAdmin)
            {
                hasAccess = true;
            }

            if (roleName == "ProjectManager" && isOwner)
            {
                hasAccess = true;
            }

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("Bu projede görev oluşturma yetkiniz bulunmamaktadır.");
            }

            if (request.DueDate.HasValue && request.DueDate.Value < project.StartDate)
            {
                throw new Exception("Son teslim tarihi proje başlangıç tarihinden önce olamaz.");
            }

            if (request.AssignedToUserId.HasValue)
            {
                var assignedMember = await _projectRepository.GetMemberAsync(
                    request.ProjectId,
                    request.AssignedToUserId.Value);

                if (assignedMember == null)
                {
                    throw new Exception("Atanan kullanıcı ilgili projenin üyesi olmalıdır.");
                }
            }

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                ProjectId = request.ProjectId,
                AssignedToUserId = request.AssignedToUserId,
                CreatedByUserId = createdByUserId,
                Status = TaskItemStatus.Todo,
                Priority = request.Priority,
                DueDate = request.DueDate,
                EstimatedHours = request.EstimatedHours,
                CreatedAt = DateTime.UtcNow,
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            return new TaskDetailDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                ProjectId = task.ProjectId,
                AssignedToUserId = task.AssignedToUserId,
                CreatedByUserId = task.CreatedByUserId,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                EstimatedHours = task.EstimatedHours,
                CreatedAt = task.CreatedAt,
                CompletedAt = task.CompletedAt
            };
        }

        public async Task UpdateAsync(Guid taskId, UpdateTaskRequestDto request, Guid userId)
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

            bool isAdmin = user.Role == ProjectManagementSystem.Enums.UserRole.Admin;
            bool isOwner = project.OwnerId == userId;
            string roleName = user.Role.ToString();
            bool hasAccess = false;

            if (isAdmin)
            {
                hasAccess = true;
            }

            if (roleName == "ProjectManager" && isOwner)
            {
                hasAccess = true;
            }

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("Bu görevi güncelleme yetkiniz bulunmamaktadır.");
            }

            if (request.AssignedToUserId.HasValue && request.AssignedToUserId != task.AssignedToUserId)
            {
                var assignedMember = await _projectRepository.GetMemberAsync(
                    task.ProjectId,
                    request.AssignedToUserId.Value);

                if (assignedMember == null)
                {
                    throw new Exception("Atanan kullanıcı ilgili projenin üyesi olmalıdır.");
                }
            }

            if (task.Priority != request.Priority)
            {
                var history = new TaskHistory
                {
                    Id = Guid.NewGuid(),
                    TaskId = task.Id,
                    ChangedByUserId = userId,
                    ChangeType = ProjectManagementSystem.Enums.ChangeType.PriorityChanged,
                    OldValue = task.Priority.ToString(),
                    NewValue = request.Priority.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                await _taskRepository.AddHistoryAsync(history);
            }

            if (task.Status != request.Status)
            {
                var history = new TaskHistory
                {
                    Id = Guid.NewGuid(),
                    TaskId = task.Id,
                    ChangedByUserId = userId,
                    ChangeType = ProjectManagementSystem.Enums.ChangeType.StatusChanged,
                    OldValue = task.Status.ToString(),
                    NewValue = request.Status.ToString(),
                    CreatedAt = DateTime.UtcNow
                };
                await _taskRepository.AddHistoryAsync(history);
            }

            if (task.AssignedToUserId != request.AssignedToUserId)
            {
                var history = new TaskHistory
                {
                    Id = Guid.NewGuid(),
                    TaskId = task.Id,
                    ChangedByUserId = userId,
                    ChangeType = ProjectManagementSystem.Enums.ChangeType.AssignedUserChanged,
                    OldValue = task.AssignedToUserId?.ToString(),
                    NewValue = request.AssignedToUserId?.ToString(),
                    CreatedAt = DateTime.UtcNow
                };
                await _taskRepository.AddHistoryAsync(history);
            }


            if (request.Status == ProjectManagementSystem.Enums.TaskItemStatus.Done)
            {
                task.CompletedAt = DateTime.UtcNow;
            }
            else
            {
                task.CompletedAt = null;
            }

            task.Title = request.Title;
            task.Description = request.Description;
            task.Priority = request.Priority;
            task.Status = request.Status;
            task.AssignedToUserId = request.AssignedToUserId;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();

        }

        public async Task ArchiveAsync(Guid taskId, Guid userId)
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

            bool isAdmin = user.Role == ProjectManagementSystem.Enums.UserRole.Admin;
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


            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("Bu görevi silme yetkiniz bulunmamaktadır.");
            }

            task.IsDeleted = true;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();
        }


        public async Task<TaskDetailDto> GetByIdAsync(Guid id, Guid currentUserId)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
            {
                throw new Exception("Görev bulunamadı.");
            }

            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            bool isAdmin = user.Role == ProjectManagementSystem.Enums.UserRole.Admin;
            var project = await _projectRepository.GetByIdAsync(task.ProjectId);
            bool isOwner = project != null && project.OwnerId == currentUserId;

            if (!isAdmin && !isOwner)
            {
                await ValidateUserIsProjectMember(currentUserId, task.ProjectId);

                if (task.AssignedToUserId != currentUserId)
                {
                    throw new UnauthorizedAccessException("Bu görevi görüntüleme yetkiniz bulunmamaktadır.");
                }
            }

            return new TaskDetailDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                ProjectId = task.ProjectId,
                AssignedToUserId = task.AssignedToUserId,
                CreatedByUserId = task.CreatedByUserId,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                EstimatedHours = task.EstimatedHours,
                CreatedAt = task.CreatedAt,
                CompletedAt = task.CompletedAt
            };
        }

        public async Task<List<TaskListDto>> GetAllAsync(
            Guid? projectId,
            ProjectManagementSystem.Enums.TaskItemStatus? status,
            Guid? assignedToUserId,
            TaskPriority? priority,
            DateTime? dueBefore,
            DateTime? dueAfter,
            string? sortBy,
            string? sortDirection,
            int page,
            int pageSize,
            Guid currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(currentUserId);
            if(user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            bool isAdmin = user.Role == ProjectManagementSystem.Enums.UserRole.Admin;

            if (projectId.HasValue)
            {
                var project = await _projectRepository.GetByIdAsync(projectId.Value);

                if (project == null || project.IsDeleted)
                {
                    return new List<TaskListDto>();
                }

                if (!isAdmin && project.OwnerId !=currentUserId)
                {
                    var member = await _projectRepository.GetMemberAsync(projectId.Value, currentUserId);
                    if (member == null)
                    {
                        throw new UnauthorizedAccessException("Bu projenin görevlerini görüntüleme yetkiniz bulunmamaktadır.");
                    }
                    assignedToUserId = currentUserId;
                }
            }
            else
            {
                if (!isAdmin)
                {
                    assignedToUserId = currentUserId;
                }
            }

                var tasks = await _taskRepository.GetAllAsync(
                    projectId,
                    status,
                    assignedToUserId,
                    priority,
                    dueBefore,
                    dueAfter,
                    sortBy,
                    sortDirection,
                    page,
                    pageSize);

            return tasks.Select(x => new TaskListDto
            {
                Id = x.Id,
                Title = x.Title,
                ProjectId = x.ProjectId,
                Status = x.Status,
                AssignedToUserId = x.AssignedToUserId,
                Priority = x.Priority,
                DueDate = x.DueDate,
            }).ToList();
        }

        public async Task UpdateStatusAsync(Guid taskId, TaskItemStatus newStatus, Guid userId)
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
                throw new UnauthorizedAccessException("Viewer rolündeki kullanıcılar görev durumunu değiştiremez.");
            }

            var project = await _projectRepository.GetByIdAsync(task.ProjectId);
            if (project == null)
            {
                throw new Exception("Proje bulunamadı.");
            }

            bool isAdmin = user.Role == ProjectManagementSystem.Enums.UserRole.Admin;
            bool isProjectOwner = project.OwnerId == userId;
            string roleName = user.Role.ToString();

            bool hasAccess = false;

            if (isAdmin)
            {
                hasAccess = true;
            }

            else if (roleName == "ProjectManager" && isProjectOwner)
            {
                hasAccess = true;
            }

            else
            {

                await ValidateUserIsProjectMember(userId, task.ProjectId);

                if (task.AssignedToUserId == userId)
                {
                    hasAccess = true;
                }
                else
                {
                    throw new UnauthorizedAccessException("Yalnızca kendi üzerinize atanan görevlerin durumunu değiştirebilirsiniz.");
                }
            }

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("Bu görevin durumunu değiştirme yetkiniz bulunmamaktadır.");
            }

            if (task.Status != newStatus)
            {
                var history = new TaskHistory
                {
                    Id = Guid.NewGuid(),
                    TaskId = task.Id,
                    ChangedByUserId = userId,
                    ChangeType = ChangeType.StatusChanged,
                    OldValue = task.Status.ToString(),
                    NewValue = newStatus.ToString(),
                    CreatedAt = DateTime.UtcNow
                };
                await _taskRepository.AddHistoryAsync(history);

                if (newStatus == TaskItemStatus.Done)
                {
                    task.CompletedAt = DateTime.UtcNow;
                }
                else
                {
                    task.CompletedAt = null;
                }

                task.Status = newStatus;
                task.UpdatedAt = DateTime.UtcNow;

                await _taskRepository.UpdateAsync(task);
                await _taskRepository.SaveChangesAsync();
            }
        }
    }
}