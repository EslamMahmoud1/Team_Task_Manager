using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.Role;

namespace Team_Task_Manager.Services.Interfaces
{
    public interface IRoleService
    {
        public Task<UserRoles> GetRoleById (long roleId);
        public Task<long> CreateRole (string roleName);
        public Task DeleteRole (long roleId);
        public Task<UserRoles> EditRole (UserRoles EditRole);
        public List<UserRoles> GetAllRoles();
        public Task<List<Permission>> GetRolePermissions(long roleId);

    }
}
