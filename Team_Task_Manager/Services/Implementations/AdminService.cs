using Microsoft.EntityFrameworkCore;
using Team_Task_Manager.Data;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.Role;
using Team_Task_Manager.Services.Interfaces;

namespace Team_Task_Manager.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly TaskAppDbContext _context;
        public AdminService(TaskAppDbContext context)
        {
            _context = context;
        }
        
        public async Task AssignRolePermissions(string roleName, List<Permission> permissions)
        {
            var role = await _context.TaskUserRoles.Where(r => r.Name == roleName).FirstOrDefaultAsync();

            foreach (var permission in permissions)
            {
                await _context.RolePermissions.AddAsync(new RolePermission { Role = role, Permission = permission });
            }
            await _context.SaveChangesAsync();
        }

        public List<Permission> GetAllPermissions()
        {
            return _context.Permissions.ToList();
        }

        public List<Permission> GetAllPermissionsByIds(List<long> SelectedPermissionIds)
        {
            return GetAllPermissions().Where(p => SelectedPermissionIds.Contains(p.Id)).ToList();
        }
    }
}
