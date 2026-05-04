using System.Security.Claims;
using Team_Task_Manager.Models.Entities.Permissions;

namespace Team_Task_Manager.Shared
{
    public class PermissionHelper
    {
        public static bool HasPermission(List<string> userClaims, PermissionName permission)
        {
            return userClaims.Any(c =>
                c == permission.ToString());
        }

        public static bool HasAnyPermission(List<string> userClaims, PermissionName[] permissions)
        {
            return permissions.Any(permission => HasPermission(userClaims, permission));
        }
    }
}
