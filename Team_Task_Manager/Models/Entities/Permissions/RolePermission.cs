using Team_Task_Manager.Models.Entities.Role;

namespace Team_Task_Manager.Models.Entities.Permissions
{
    public class RolePermission
    {
        public long RoleId { get; set; }
        public UserRoles Role { get; set; } = new UserRoles();
        public long PermissionId { get; set; }
        public Permission Permission { get; set; } = new Permission();
    }
}
