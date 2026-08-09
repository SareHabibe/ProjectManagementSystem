using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagementSystem.Enums;
using ProjectManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Data
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await context.Database.MigrateAsync();

            if (!await context.Users.AnyAsync())
            {
                var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();

                //ADMİN KULLANICI
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@project.com",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");


                //PROJECT MANAGER KULLANICILAR
                var pmUser = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Ahmet",
                    LastName = "Yılmaz",
                    Email = "projectManager@project.com",
                    Role = UserRole.ProjectManager,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                pmUser.PasswordHash = passwordHasher.HashPassword(pmUser, "Pm123!");


                var pmUser2 = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Ceren",
                    LastName = "Kara",
                    Email = "projectManager2@project.com",
                    Role = UserRole.ProjectManager,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                pmUser2.PasswordHash = passwordHasher.HashPassword(pmUser2, "Pm123!");


                //TEAM MEMBER KULLANICILAR
                var memberUser = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Mehmet",
                    LastName = "Demir",
                    Email = "member@project.com",
                    Role = UserRole.TeamMember,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                memberUser.PasswordHash = passwordHasher.HashPassword(memberUser, "member123!");

                var memberUser2 = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Selin",
                    LastName = "Karaca",
                    Email = "member2@project.com",
                    Role = UserRole.TeamMember,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                memberUser2.PasswordHash = passwordHasher.HashPassword(memberUser2, "member123!");

                //VİEWER KULLANICILAR
                var viewerUser = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Ayşe",
                    LastName = "Kaya",
                    Email = "viewer@project.com",
                    Role = UserRole.Viewer,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                viewerUser.PasswordHash = passwordHasher.HashPassword(viewerUser, "Viewer123!");

                await context.Users.AddRangeAsync(adminUser, pmUser, pmUser2, memberUser, memberUser2, viewerUser);
                await context.SaveChangesAsync();
            }


            if (!await context.Projects.AnyAsync())
            {
                var admin = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@project.com");
                var pm = await context.Users.FirstOrDefaultAsync(u => u.Email == "projectManager@project.com");
                var pm2 = await context.Users.FirstOrDefaultAsync(u => u.Email == "projectManager2@project.com");
                var member = await context.Users.FirstOrDefaultAsync(u => u.Email == "member@project.com");
                var member2 = await context.Users.FirstOrDefaultAsync(u => u.Email == "member2@project.com");
                var viewer = await context.Users.FirstOrDefaultAsync(u => u.Email == "viewer@project.com");

                if (admin != null && pm != null && pm2 != null && member != null && member2 != null && viewer != null)
                {

                    var adminProject = new Project
                    {
                        Id = Guid.NewGuid(),
                        Name = "Admin Yönetim Projesi",
                        Description = "Admin tarafından yönetilen örnek proje.",
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(3),
                        Status = ProjectStatus.Active,
                        OwnerId = admin.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    var pmProject = new Project
                    {
                        Id = Guid.NewGuid(),
                        Name = "Geliştirme Projesi",
                        Description = "Project Manager tarafından yönetilen geliştirme projesi.",
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(3),
                        Status = ProjectStatus.Active,
                        OwnerId = pm.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    var pm2Project = new Project
                    {
                        Id = Guid.NewGuid(),
                        Name = "Mobil Uygulama Projesi",
                        Description = "İkinci Project Manager'ın yönettiği mobil uygulama projesi.",
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(4),
                        Status = ProjectStatus.Active,
                        OwnerId = pm2.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    await context.Projects.AddRangeAsync(adminProject, pmProject, pm2Project);
                    await context.SaveChangesAsync();

                    var projectMembers = new List<ProjectMember>
                    {
                        // 1. Geliştirme Projesi Üyeleri (PM1 projesi)
                        new ProjectMember { Id = Guid.NewGuid(),ProjectId = pmProject.Id,UserId = pm.Id,Role = ProjectMemberRole.Manager,IsActive = true},
                    
                        new ProjectMember { Id = Guid.NewGuid(),ProjectId = pmProject.Id,UserId = member.Id,Role = ProjectMemberRole.Member,IsActive = true },

                        new ProjectMember { Id = Guid.NewGuid(),ProjectId = pmProject.Id, UserId = viewer.Id, Role = ProjectMemberRole.Viewer, IsActive = true},
                       
                        
                        // 2. Admin Yönetim Projesi Üyeleri
                        new ProjectMember { Id = Guid.NewGuid(),ProjectId = adminProject.Id,UserId = admin.Id,Role = ProjectMemberRole.Manager,IsActive = true },
                            
                        new ProjectMember{ Id = Guid.NewGuid(),ProjectId = adminProject.Id,UserId = member2.Id,Role = ProjectMemberRole.Member,IsActive = true},
                       
                        new ProjectMember{ Id = Guid.NewGuid(),ProjectId = adminProject.Id, UserId = viewer.Id,Role = ProjectMemberRole.Viewer, IsActive = true },
                        

                        // 3. Mobil Uygulama Projesi Üyeleri (PM2 projesi)
                        new ProjectMember { Id = Guid.NewGuid(),ProjectId = pm2Project.Id,UserId = pm2.Id,Role = ProjectMemberRole.Manager,IsActive = true },
                        
                        new ProjectMember { Id = Guid.NewGuid(), ProjectId = pm2Project.Id, UserId = member2.Id, Role = ProjectMemberRole.Member, IsActive = true },
                       
                        new ProjectMember { Id = Guid.NewGuid(), ProjectId = pm2Project.Id, UserId = viewer.Id, Role = ProjectMemberRole.Viewer, IsActive = true },
                    };

                    await context.ProjectMembers.AddRangeAsync(projectMembers);
                    await context.SaveChangesAsync();

                    var taskAdmin1 = new TaskItem
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = adminProject.Id,
                        Title = "Sistem Güvenlik Denetimi",
                        Description = "Admin paneli yetkilendirme testleri.",
                        Priority = TaskPriority.High,
                        Status = TaskItemStatus.Done,
                        AssignedToUserId = member2.Id,
                        CreatedByUserId = admin.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        IsDeleted = false
                    };

                    var taskPm1 = new TaskItem
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = pmProject.Id,
                        Title = "Veritabanı Şemasının Çıkarılması",
                        Description = "EF Core migration'larının ayarlanması.",
                        Priority = TaskPriority.High,
                        Status = TaskItemStatus.InProgress,
                        AssignedToUserId = member.Id,
                        CreatedByUserId = pm.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    var taskPm2 = new TaskItem
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = pmProject.Id,
                        Title = "Sistem Mimarisi İncelemesi",
                        Description = "PM seviyesinde mimari kontrol.",
                        Priority = TaskPriority.Medium,
                        Status = TaskItemStatus.Todo,
                        AssignedToUserId = pm.Id,
                        CreatedByUserId = pm.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    var taskPm2Proj1 = new TaskItem
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = pm2Project.Id,
                        Title = "Mobil API Entegrasyonu",
                        Description = "Flutter istemcisi için endpoint'lerin bağlanması.",
                        Priority = TaskPriority.High,
                        Status = TaskItemStatus.InProgress,
                        AssignedToUserId = member2.Id,
                        CreatedByUserId = pm2.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    await context.Tasks.AddRangeAsync(taskAdmin1, taskPm1, taskPm2, taskPm2Proj1);
                    await context.SaveChangesAsync();

                    var comments = new List<Comment>
                    {
                        new Comment
                        {
                            Id = Guid.NewGuid(),
                            TaskId = taskPm1.Id,
                            UserId = member.Id,
                            Content = "Migration dosyaları oluşturuldu, inceleyebilir misiniz?",
                            CreatedAt = DateTime.UtcNow.AddHours(-2)
                        },
                        new Comment
                        {
                            Id = Guid.NewGuid(),
                            TaskId = taskPm1.Id,
                            UserId = pm.Id,
                            Content = "Ellerine sağlık, gayet iyi görünüyor. Bağlantı string'lerini kontrol edip test edebiliriz.",
                            CreatedAt = DateTime.UtcNow.AddHours(-1)
                        },
                        new Comment
                        {
                            Id = Guid.NewGuid(),
                            TaskId = taskPm2Proj1.Id,
                            UserId = member2.Id,
                            Content = "Auth endpoint'leri entegre edildi, token testleri yapılıyor.",
                            CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                        }
                    };

                    await context.Comments.AddRangeAsync(comments);

                    var historyLogs = new List<TaskHistory>
                    {

                        new TaskHistory
                        {
                            Id = Guid.NewGuid(),
                            TaskId = taskAdmin1.Id,
                            ChangedByUserId = admin.Id,
                            ChangeType = ChangeType.StatusChanged,
                            OldValue = "InProgress",
                            NewValue = "Done",
                            Description = "Admin görevi tamamladı.",
                            CreatedAt = DateTime.UtcNow.AddHours(-5)
                        },

                        new TaskHistory
                        {
                            Id = Guid.NewGuid(),
                            TaskId = taskPm1.Id,
                            ChangedByUserId = pm.Id,
                            ChangeType = ChangeType.AssignedUserChanged,
                            OldValue = null,
                            NewValue = "Todo",
                            Description = "Görev oluşturuldu.",
                            CreatedAt = DateTime.UtcNow.AddHours(-4)
                        },

                        new TaskHistory
                        {
                            Id = Guid.NewGuid(),
                            TaskId = taskPm1.Id,
                            ChangedByUserId = member.Id,
                            ChangeType = ChangeType.StatusChanged,
                            OldValue = "Todo",
                            NewValue = "InProgress",
                            Description = "Görev durumu güncellendi.",
                            CreatedAt = DateTime.UtcNow.AddHours(-1)
                        },

                        new TaskHistory
                        {
                            Id = Guid.NewGuid(),
                            TaskId = taskPm2Proj1.Id,
                            ChangedByUserId = pm2.Id,
                            ChangeType = ChangeType.AssignedUserChanged,
                            OldValue = null,
                            NewValue = "InProgress",
                            Description = "Mobil görev atandı.",
                            CreatedAt = DateTime.UtcNow.AddHours(-2)
                        }
                    };

                    await context.TaskHistories.AddRangeAsync(historyLogs);

                    var timeLogs = new List<TaskTimeLog>
                    {

                        new TaskTimeLog
                        {
                            Id = Guid.NewGuid(),
                            TaskId = taskAdmin1.Id,
                            UserId = member2.Id,
                            Hours = 1.5m,
                            Description = "Yetkilendirme testleri yapıldı.",
                            WorkDate = DateTime.UtcNow.Date,
                            CreatedAt = DateTime.UtcNow
                        },

                        new TaskTimeLog
                        {
                            Id = Guid.NewGuid(),
                            TaskId = taskPm1.Id,
                            UserId = member.Id,
                            Hours = 2.0m,
                            Description = "Migration dosyaları hazırlandı.",
                            WorkDate = DateTime.UtcNow.Date,
                            CreatedAt = DateTime.UtcNow
                        },

                        new TaskTimeLog
                        {
                            Id = Guid.NewGuid(),
                            TaskId = taskPm2Proj1.Id,
                            UserId = member2.Id,
                            Hours = 3.5m,
                            Description = "Token servisleri yazıldı.",
                            WorkDate = DateTime.UtcNow.Date,
                            CreatedAt = DateTime.UtcNow
                        }
                    };

                    await context.TaskTimeLogs.AddRangeAsync(timeLogs);

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}