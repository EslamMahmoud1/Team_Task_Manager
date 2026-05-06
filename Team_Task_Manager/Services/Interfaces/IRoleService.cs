using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.Role;
using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.Shared;

namespace Team_Task_Manager.Services.Interfaces
{
    public interface IRoleService
    {
        public Task<UserRoles> GetRoleById(long roleId);
        public Task<Result<long>> CreateRole(string roleName);
        public Task<Result<UserRoles>> DeleteRole(long roleId);
        public Task<Result<UserRoles>> EditRole(string RoleName, List<long> SelectedPermissionIds);
        public Task<List<UserRoles>> GetAllRolesAsync();
        public Task<List<TaskUser>> HasUsers(long roleId);
        public Task<Result<bool>> ReassignUsersAndDelete(long roleId, long newRoleId);
        public Task<List<Permission>> GetRolePermissions(long roleId);
    }
}
