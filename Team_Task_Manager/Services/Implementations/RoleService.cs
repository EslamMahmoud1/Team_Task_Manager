using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Team_Task_Manager.Data;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.Role;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.Shared;

namespace Team_Task_Manager.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly TaskAppDbContext _dbContext;
        private readonly RoleManager<UserRoles> _roleManager;


        public RoleService(TaskAppDbContext dbContext, RoleManager<UserRoles> roleManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
        }

        public async Task<Result<long>> CreateRole(string roleName)
        {
            var role = new UserRoles { Name = roleName };

            var oldRole = _dbContext.Roles.Where(r => r.Name == roleName);
            if(oldRole.Any()) return Result<long>.Failure(new List<string> { "Role already exists." });

            await _dbContext.Roles.AddAsync(role);
            await _dbContext.SaveChangesAsync();
            return Result<long>.Success(role.Id);
        }

        public async Task DeleteRole(long roleId)
        {
            var role = await _dbContext.TaskUserRoles.FirstOrDefaultAsync(r => r.Id == roleId);
            _dbContext.TaskUserRoles.Remove(role);
            await _dbContext.SaveChangesAsync();   
        }

        public async Task<UserRoles> EditRole(UserRoles EditRole)
        {
            var role = _dbContext.Roles.Find(EditRole.Id);
            if(role is null) return new UserRoles();
            role.Name = EditRole.Name;
            _dbContext.Roles.Update(role);
            await _dbContext.SaveChangesAsync();
            return role;
        }
        public List<UserRoles> GetAllRoles()
        {
            return _dbContext.TaskUserRoles.ToList();
        }

        public async Task<UserRoles> GetRoleById(long roleId)
        {
            return await _dbContext.TaskUserRoles.FindAsync(roleId);
        }
        public async Task<List<Permission>> GetRolePermissions(long roleId)
        {
            var rolePermissions = await _dbContext.RolePermissions.Where(rp => rp.RoleId == roleId)
                .Select(p => p.Permission).ToListAsync();

            return rolePermissions;
        }
    }
}
