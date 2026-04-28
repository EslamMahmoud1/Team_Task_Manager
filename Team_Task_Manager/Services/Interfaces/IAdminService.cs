using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.Role;

namespace Team_Task_Manager.Services.Interfaces
{
    public interface IAdminService
    {
        public List<Permission> GetAllPermissions();
        public List<Permission> GetAllPermissionsByIds(List<long> SelectedPermissionIds);
        public Task AssignRolePermissions(string roleName , List<Permission> permissions);
    }
}
