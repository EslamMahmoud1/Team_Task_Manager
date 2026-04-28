using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Team_Task_Manager.Data;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.Role;
using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.Shared;
using Team_Task_Manager.ViewModels.Role;

namespace Team_Task_Manager.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<UserRoles> _roleManager;
        private readonly UserManager<TaskUser> _userManager;
        private readonly TaskAppDbContext _context;

        public RoleService(RoleManager<UserRoles> roleManager, UserManager<TaskUser> userManager, TaskAppDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
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
            if(role is null) return Result<UserRoles>.Failure(new List<string> { "Role not found." });

            var usersInRole = await _context.TaskUsers.Where(u => u.UserRoleId == id).ToListAsync();
            if (usersInRole.Any()) return Result<UserRoles>.Failure(new List<string> { "Cannot delete role with assigned users." });

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded) return Result<UserRoles>.Failure(new List<string> { string.Join(", ", result.Errors.Select(e => e.Description)) });

            return Result<UserRoles>.Success(role);
        }

        public async Task<Result<UserRoles>> EditRole(RoleEditViewModel EditRole)
        {
            var role = await _roleManager.FindByIdAsync(EditRole.Id.ToString());
            if (role is null) return Result<UserRoles>.Failure(new List<string> { "Role not found." });

            role.Name = EditRole.Name;
            role.NormalizedName = EditRole.Name.ToUpper();

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded) return Result<UserRoles>.Failure(result.Errors.Select(e => e.Description).ToList());

            return Result<UserRoles>.Success(role);
        }
        public async Task<List<UserRoles>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
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
    }
}
