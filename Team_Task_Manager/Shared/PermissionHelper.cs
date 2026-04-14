using System.Security.Claims;
using Team_Task_Manager.Models.Entities.Permissions;

namespace Team_Task_Manager.Shared
{
    public class PermissionHelper
    {
        public static bool HasPermission(ClaimsPrincipal user, PermissionName permission)
        {
            return user.Claims.Any(c =>
                c.Type == "Permission" && c.Value == permission.ToString());
        }

        public static bool HasAnyPermission(ClaimsPrincipal user, PermissionName[] permissions)
        {
            return permissions.Any(permission => HasPermission(user, permission));
        }
    }
}
