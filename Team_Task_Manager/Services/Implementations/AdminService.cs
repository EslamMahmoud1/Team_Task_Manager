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
        
        public async Task AssignRolePermissions(long roleId, List<Permission> permissions)
        {
            var role = await _context.TaskUserRoles.FindAsync(roleId);

            var permissionNames = permissions.Select(p => p.Name).ToList();
            var dbPermissions = await _context.Permissions
                                    .Where(p => permissionNames.Contains(p.Name))
                                    .ToListAsync();

            var old = _context.RolePermissions.Where(r => r.RoleId == roleId);
            _context.RolePermissions.RemoveRange(old);

            foreach (var permission in dbPermissions)
            {
                await _context.RolePermissions.AddAsync(new RolePermission { Role = role, Permission = permission });
            }
            await _context.SaveChangesAsync();
        }

        public ICollection<Permission> GetAllPermissions()
        {
            return _context.Permissions.ToList();
        }

        public ICollection<UserRoles> GetAllRoles()
        {
            return _context.TaskUserRoles.ToList();
        }
        
        public async Task<List<Permission>> GetRolePermissions(long roleId)
        {
            var rolePermissions = await _context.RolePermissions.Where(rp => rp.RoleId == roleId)
                .Select(p => p.Permission).ToListAsync();

            return rolePermissions;
        }
    }
}
