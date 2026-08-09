using ProjectManagementSystem.DTOs.Projects;
using ProjectManagementSystem.Enums;
using ProjectManagementSystem.Repositories.Interfaces;
using ProjectManagementSystem.Services.Interfaces;
using ProjectManagementSystem.Models;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace ProjectManagementSystem.Services.Implementations
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;

        public ProjectService(IProjectRepository projectRepository, IUserRepository userRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            
        }

        public async Task CreateAsync(CreateProjectRequestDto request, Guid ownerId, string userRole)
        {

            if (userRole !="Admin" && userRole !="ProjectManager")
            {
                throw new Exception("Proje sahibi yalnızca Admin veya ProjectManager rolünde olabilir.");
            }
            
            if (request.EndDate.HasValue && request.EndDate.Value < request.StartDate)
            {
                throw new Exception("Bitiş tarihi başlangıç tarihinden önce olamaz.");
            }
            
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status,
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();

        }

        public async Task UpdateAsync(Guid projectId, UpdateProjectRequestDto request, Guid userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                throw new Exception("Proje bulunamadı.");
            }

            var currentUser = await _userRepository.GetByIdAsync(userId);

            if (currentUser == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            bool isAdmin = currentUser.Role == ProjectManagementSystem.Enums.UserRole.Admin;

            if (!isAdmin && project.OwnerId !=userId)
            {
                throw new Exception("Projeyi güncelleme yetkiniz yok.");

            }
            if (request.EndDate.HasValue && request.EndDate < request.StartDate)
            {
                throw new Exception("Bitiş tarihi başlangıç tarihinden önce olamaz.");
            }

            project.Name = request.Name;
            project.Description = request.Description;
            project.StartDate = request.StartDate;
            project.EndDate = request.EndDate;
            project.Status = request.Status;

            await _projectRepository.UpdateAsync(project);
            await _projectRepository.SaveChangesAsync();

        }

        public async Task<List<ProjectListDto>> GetAllAsync(
            ProjectStatus? status,
            Guid? ownerId,
            Guid UserId,
            bool isAdmin,
            int page,
            int pageSize)
        {
            var projects = await _projectRepository.GetAllAsync(
                status,
                ownerId,
                UserId,
                isAdmin,
                page,
                pageSize);



            if(!isAdmin && (projects == null || !projects.Any()))
            {
                throw new Exception("Eklendiğiniz proje bulunmamaktadır.");
            }

            return projects.Select(x => new ProjectListDto
            {
                Id = x.Id,
                Name = x.Name,
                Status = x.Status,
                OwnerId = x.OwnerId,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
            }).ToList();
        }


        public async Task<List<ProjectMemberListDto>> GetProjectMemberAsync(Guid projectId, Guid userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                throw new Exception("Proje bulunamadı.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            bool isAdmin = user.Role == ProjectManagementSystem.Enums.UserRole.Admin;
            bool isOwner = project.OwnerId == userId;
            bool isActiveMember = project.ProjectMembers != null && project.ProjectMembers.Any(pm => pm.UserId == userId && pm.IsActive);
            string roleName = user.Role.ToString();
            bool hasAccess = false;

            if(isAdmin)
            {
                hasAccess = true;
            }

            else if(roleName=="ProjectManager" && (isOwner||isActiveMember))
            {
                hasAccess = true;
            }

            else if((roleName=="TeamMember" || roleName=="Viewer") && isActiveMember)
            {
                hasAccess = true;
            }

            if(!hasAccess)
            {
                throw new UnauthorizedAccessException("Bu projenin üyelerini görüntüleme yetkiniz bulunmamaktadır. ");
            }

            var members = await _projectRepository.GetProjectMemberProjectIdAsync(projectId);

            return members.Select(m => new ProjectMemberListDto
            {
                UserId = m.UserId,
                FirstName = m.User?.FirstName,
                LastName = m.User?.LastName,
                Role = m.Role
            }).ToList();
        }

          public async Task<ProjectDetailDto> GetByIdAsync(Guid projectId, Guid userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                throw new Exception("Proje bulunamadı.");
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if(user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            bool isAdmin = user.Role == ProjectManagementSystem.Enums.UserRole.Admin;
            bool isOwner = project.OwnerId == userId;
            bool isActiveMember = project.ProjectMembers != null && project.ProjectMembers.Any
                                 (pm => pm.UserId == userId && pm.IsActive);
            string roleName = user.Role.ToString();

            bool hasAccess = false;

            if (isAdmin)
            {
                hasAccess = true;
            }

            else if (roleName == "ProjectManager" && (isOwner || isActiveMember))
            {
                hasAccess = true;
            }

            else if ((roleName == "TeamMember" || roleName == "Viewer") && isActiveMember)
            {
                hasAccess = true;
            }

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("Bu projenin detaylarını görüntüleme yetkiniz bulunmamaktadır.");
            }

                return new ProjectDetailDto
                {
                    Id = projectId,
                    Name = project.Name,
                    Description = project.Description,
                    Status = project.Status,
                    OwnerId = project.OwnerId,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    CreatedAt = project.CreatedAt,
                };
        }


        public async Task ArchiveAsync(Guid projectId, Guid userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                throw new Exception("Proje bulunamadı.");
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if(user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            bool isAdmin = user.Role == ProjectManagementSystem.Enums.UserRole.Admin;
            bool isOwner = project.OwnerId == userId;
            string roleName = user.Role.ToString();

            bool hasAccess = false;
            if (isAdmin)
            {
                hasAccess = true;
            }

            else if (roleName == "ProjectManager" && isOwner)
            {
                hasAccess = true;
            }

            if(!hasAccess)
            {
                throw new UnauthorizedAccessException("Bu projeyi silme yetkiniz bulunmamaktadır.");
            }
                project.IsDeleted = true;

            await _projectRepository.UpdateAsync(project);
            await _projectRepository.SaveChangesAsync();
        }

        public async Task AddMemberAsync(
            Guid projectId,
            AddProjectMemberRequestDto request, Guid userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                throw new Exception("Proje bulunamadı.");
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if ( user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            bool isAdmin = user.Role == ProjectManagementSystem.Enums.UserRole.Admin;
            bool isOwner = project.OwnerId == userId;
            string roleName = user.Role.ToString();

            bool hasAccess = false;

            if (isAdmin)
            {
                hasAccess = true;
            }

            if(roleName == "ProjectManager" && isOwner)
            {
                hasAccess = true;
            }

            if(!hasAccess)
            {
                throw new UnauthorizedAccessException("Bu projeye üye ekleme yetkiniz bulunmamaktadır.");
            }

            var existingMember = await _projectRepository
                .GetMemberAsync(projectId, request.UserId);

            if (existingMember != null)
            {
                throw new Exception("Kullanıcı zaten projeye eklenmiş.");
            }

            var member = new ProjectMember
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                UserId = request.UserId,
                Role = request.Role,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _projectRepository.AddMemberAsync(member);
            await _projectRepository.SaveChangesAsync();
        }

       
        public async Task RemoveMemberAsync(Guid projectId,Guid memberId,Guid userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
                
            if (project == null)
            {
                throw new Exception("Proje bulunamadı.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            bool isAdmin = user.Role == ProjectManagementSystem.Enums.UserRole.Admin;
            bool isOwner = project.OwnerId == userId;
            string roleName = user.Role.ToString();

            bool hasAccess = false;

            if(isAdmin)
            {
                hasAccess = true;
            }

            else if(roleName == "ProjectManager" && isOwner)
            {
                hasAccess = true;
            }

            if(!hasAccess)
            {
                throw new UnauthorizedAccessException("Bu projede üye silme yetkiniz bulunmamaktadır.");
            }


            var member = await _projectRepository
                .GetMemberAsync(projectId, memberId);

            if (member == null)
            {
                throw new Exception("Proje üyesi bulunamadı.");
            }

            await _projectRepository.RemoveMemberAsync(member);
            await _projectRepository.SaveChangesAsync();
        }
    }
}
