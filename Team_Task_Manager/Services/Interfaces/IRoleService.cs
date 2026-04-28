using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.Role;
using Team_Task_Manager.Shared;
using Team_Task_Manager.ViewModels.Role;

namespace Team_Task_Manager.Services.Interfaces
{
    public interface IRoleService
    {
        public Task<UserRoles> GetRoleById (long roleId);
        public Task<Result<long>> CreateRole (string roleName);
        public Task<Result<UserRoles>> DeleteRole (long roleId);
        public Task<Result<UserRoles>> EditRole (RoleEditViewModel EditRole);
        public Task<List<UserRoles>> GetAllRolesAsync();
        public Task<List<Permission>> GetRolePermissions(long roleId);

    }
}
