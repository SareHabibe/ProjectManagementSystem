using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Data;
using ProjectManagementSystem.Enums;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories.Interfaces;



namespace ProjectManagementSystem.Repositories.Implementations
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;
        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
        }

        public async Task<Project?> GetByIdAsync(Guid id)
        {
            return await _context.Projects
                .Include(p => p.ProjectMembers)
                .FirstOrDefaultAsync(x => x.Id == id  && !x.IsDeleted);
        }

        public async Task<List<Project>> GetAllAsync(
            ProjectStatus? status,
            Guid? ownerId,
            Guid UserId,
            bool isAdmin,
            int page,
            int pageSize)
        {
            var query = _context.Projects
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(p => p.OwnerId == UserId ||
                p.ProjectMembers.Any(pm => pm.UserId == UserId && pm.IsActive));
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (ownerId.HasValue)
            {
                query = query.Where(x => x.OwnerId == ownerId.Value);
            }

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<ProjectMember>> GetProjectMemberProjectIdAsync(Guid projectId)
        {
            return await _context.ProjectMembers
                .Include(pm => pm.User)
                .Where(pm => pm.ProjectId == projectId && !pm.User.IsDeleted)
                .ToListAsync();
        }
        public Task UpdateAsync(Project project)
        {
            _context.Projects.Update(project);

            return Task.CompletedTask;
        }

        public async Task AddMemberAsync(ProjectMember projectMember)
        {
            await _context.ProjectMembers.AddAsync(projectMember);
        }

        public async Task<ProjectMember?> GetMemberAsync(Guid projectId, Guid userId)
        {
            return await _context.ProjectMembers
                .FirstOrDefaultAsync(x =>
                x.ProjectId == projectId &&
                x.UserId == userId);
        }

        public Task RemoveMemberAsync(ProjectMember member)
        {
            _context.ProjectMembers .Remove(member);

            return Task.CompletedTask;
        }
       
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
