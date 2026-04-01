using System.Data;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.Role;
using Team_Task_Manager.Models.Entities.User;

namespace Team_Task_Manager.Data.Seeding
{
    public static class DataSeeding
    {
        public static async Task Seed(TaskAppDbContext context)
        {
            if (!context.TaskUserRoles.Any())
            {
                var roles = new List<UserRoles>
                {
                     new UserRoles { Name = "Admin" },
                     new UserRoles { Name = "ActionUser" },
                     new UserRoles { Name = "NormalUser" }
                };
                await context.TaskUserRoles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }
            if(!context.Permissions.Any())
            {
                var permissions = new List<Permission>
                {
                    new Permission { Name = PermissionName.CreateTask },
                    new Permission { Name = PermissionName.ViewTask},
                    new Permission { Name = PermissionName.ChangeStatus},
                    new Permission { Name = PermissionName.DeleteTask}
                };
                await context.Permissions.AddRangeAsync(permissions);
                await context.SaveChangesAsync();
            }
            if(!context.Users.Any())
            {
                var adminRole = new UserRoles() { Name = "Admin" };
                var admin = new TaskUser()
                {
                    Email = "EslamAdmin@mail.com",
                    UserName = "EslamAdmin",
                    UserRoleId = 1,
                    
                };
                await context.Users.AddAsync(admin);
                await context.SaveChangesAsync();
            };
        }
    }
}
