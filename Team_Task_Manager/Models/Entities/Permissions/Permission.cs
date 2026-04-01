using System.Text.Json.Serialization;

namespace Team_Task_Manager.Models.Entities.Permissions
{
    public class Permission
    {
        public long Id { get; set; }
        public PermissionName Name { get; set; } 
        public ICollection<RolePermission> RolePermissions { get; set; } 
    }
}
