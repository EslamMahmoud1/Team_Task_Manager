using Microsoft.AspNetCore.Identity;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.User;

namespace Team_Task_Manager.Models.Entities.Role
{
    public class UserRoles : IdentityRole<long>
    {
        public ICollection<RolePermission> RolePermissions { get; set; } 
        public ICollection<TaskUser> TaskUsers { get; set; }
    }
}
