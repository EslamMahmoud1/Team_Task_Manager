using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.Role;

namespace Team_Task_Manager.Services.Interfaces
{
    public interface IAdminService
    {
        public ICollection<Permission> GetAllPermissions();
        public ICollection<UserRoles> GetAllRoles();
        public Task AssignRolePermissions(long roleId , List<Permission> permissions);
        public Task<List<Permission>> GetRolePermissions(long roleId);

    }
}
