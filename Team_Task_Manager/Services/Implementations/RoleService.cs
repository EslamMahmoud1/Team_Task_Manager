using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Team_Task_Manager.Data;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.Role;
using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.Shared;

namespace Team_Task_Manager.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<UserRoles> _roleManager;
        private readonly TaskAppDbContext _context;

        public RoleService(RoleManager<UserRoles> roleManager, TaskAppDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<Result<long>> CreateRole(string roleName)
        {
            var role = new UserRoles { Name = roleName };

            var oldRole = await _roleManager.FindByNameAsync(roleName);
            if (oldRole is not null) return Result<long>.Failure(new List<string> { "Role already exists." });

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded) return Result<long>.Failure(result.Errors.Select(e => e.Description).ToList());

            return Result<long>.Success(role.Id);
        }

        public async Task<Result<UserRoles>> DeleteRole(long id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role is null) return Result<UserRoles>.Failure(new List<string> { "Role not found." });

            var usersInRole = await _context.TaskUsers.Where(u => u.UserRoleId == id).ToListAsync();
            if (usersInRole.Any()) return Result<UserRoles>.Failure(new List<string> { "Cannot delete role with assigned users." });

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded) return Result<UserRoles>.Failure(new List<string> { string.Join(", ", result.Errors.Select(e => e.Description)) });

            return Result<UserRoles>.Success(role);
        }

        public async Task<Result<UserRoles>> EditRole(string RoleName, List<long> SelectedPermissionIds)
        {
            var role = await _roleManager.FindByNameAsync(RoleName);
            if (role is null) return Result<UserRoles>.Failure(new List<string> { "Role not found." });

            if (SelectedPermissionIds.Any())
            {
                var roleId = role.Id;

                // 1. Get current relations
                var existing = await _context.RolePermissions.Include(rp => rp.Permission)
                    .Where(rp => rp.RoleId == roleId)
                    .ToListAsync();

                // 2. Extract existing PermissionIds
                var existingIds = existing.Select(rp => rp.PermissionId).ToList();

                // 3. Determine what to ADD
                var toAdd = SelectedPermissionIds
                    .Except(existingIds)
                    .Select(pid => new RolePermission
                    {
                        Permission = _context.Permissions.Find(pid),
                        Role = role
                    });

                // 4. Determine what to REMOVE
                var toRemove = existing
                    .Where(rp => !SelectedPermissionIds.Contains(rp.PermissionId));

                // 5. Apply changes
                _context.RolePermissions.RemoveRange(toRemove);
                await _context.RolePermissions.AddRangeAsync(toAdd);
                // 6. Save
            }

            role.Name = RoleName;
            role.NormalizedName = RoleName.ToUpper();

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded) return Result<UserRoles>.Failure(result.Errors.Select(e => e.Description).ToList());

            return Result<UserRoles>.Success(role);
        }
        public async Task<List<UserRoles>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<List<TaskUser>> HasUsers(long roleId)
        {
            var users = await _context.TaskUsers.Where(u => u.UserRoleId == roleId).ToListAsync();
            return users;
        }

        public async Task<UserRoles> GetRoleById(long roleId)
        {
            return await _roleManager.FindByIdAsync(roleId.ToString());
        }
        public async Task<List<Permission>> GetRolePermissions(long roleId)
        {
            var rolePermissions = await _context.RolePermissions.Where(rp => rp.RoleId == roleId)
                .Select(p => p.Permission).ToListAsync();

            return rolePermissions;
        }

        public async Task<Result<bool>> ReassignUsersAndDelete(long roleId, long newRoleId)
        {
            var role = await GetRoleById(roleId);
            if(role == null) return Result<bool>.Failure(new List<string> { "Role not found." });

            var newRole = await GetRoleById(newRoleId);
            if (newRole == null) return Result<bool>.Failure(new List<string> { "New role not found." });

            var users = await _context.TaskUsers.Where(u => u.UserRoleId == roleId).ToListAsync();

            foreach (var user in users)
            {
                user.UserRoleId = newRoleId;
            }
            await _context.SaveChangesAsync();

            var result = await _roleManager.DeleteAsync(role);

            return Result<bool>.Success(result.Succeeded);
        }
    }
}
