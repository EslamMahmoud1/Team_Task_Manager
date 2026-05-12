using Microsoft.EntityFrameworkCore;
using System.Data;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.Role;
using Team_Task_Manager.Models.Entities.UpdateProfile;
using Team_Task_Manager.Models.Entities.User;

namespace Team_Task_Manager.Data.Seeding
{
    public static class DataSeeding
    {
        public static async Task Seed(TaskAppDbContext context)
        {
            if(!context.Permissions.Any())
            {
                var permissions = new List<Permission>
                {
                    new Permission { Name = PermissionName.AdminPanel },
                    new Permission { Name = PermissionName.Analytics},
                    new Permission { Name = PermissionName.CRM},
                    new Permission { Name = PermissionName.Logistics},
                    new Permission { Name = PermissionName.Academy},
                    new Permission { Name = PermissionName.Withoutmenu},
                    new Permission { Name = PermissionName.Withoutnavbar},
                    new Permission { Name = PermissionName.Fluid},
                    new Permission { Name = PermissionName.Container},
                    new Permission { Name = PermissionName.Blank},
                    new Permission { Name = PermissionName.AccountSettings},
                    new Permission { Name = PermissionName.Account},
                    new Permission { Name = PermissionName.Notifications},
                    new Permission { Name = PermissionName.Connections},
                    new Permission { Name = PermissionName.Login},
                    new Permission { Name = PermissionName.Register},
                    new Permission { Name = PermissionName.ForgotPassword}
                };
                await context.Permissions.AddRangeAsync(permissions);
                await context.SaveChangesAsync();
            }

            if (!context.TaskUserRoles.Any())
            {
                await context.TaskUserRoles.AddAsync(new UserRoles() { Name = "Admin" });
                await context.SaveChangesAsync();
            }

            var existingSkillNames = await context.Skills
                .Select(skill => skill.Name)
                .ToListAsync();

            var missingSkills = Enum.GetValues<SkillType>()
                .Except(existingSkillNames)
                .Select(skillType => new Skill { Name = skillType })
                .ToList();

            if (missingSkills.Count > 0)
            {
                await context.Skills.AddRangeAsync(missingSkills);
                await context.SaveChangesAsync();
            }
            
        }
    }
}
