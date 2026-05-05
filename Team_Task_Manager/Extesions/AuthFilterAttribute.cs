using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Team_Task_Manager.Data;
using Team_Task_Manager.Models.Entities.Permissions;

public class PermissionAuthFilterAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly PermissionName _requiredPermission;
    public PermissionAuthFilterAttribute(PermissionName requiredPermission)
    {
        _requiredPermission = requiredPermission;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;

        // 1. Check authentication
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        // 2. Get DbContext
        var dbContext = httpContext.RequestServices
            .GetRequiredService<TaskAppDbContext>();

        // 3. Get UserId from claims
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var userId = long.Parse(userIdClaim);

        // 4. Get RoleId directly
        var roleId = await dbContext.TaskUsers
            .Where(u => u.Id == userId)
            .Select(u => u.UserRoleId)
            .FirstOrDefaultAsync();

        // 5. Check if permission exists for this role
        var hasPermission = await dbContext.RolePermissions
            .AnyAsync(rp =>
                rp.RoleId == roleId &&
                rp.Permission.Name == _requiredPermission);

        // 6. Deny if not allowed
        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}

public static class ClaimName
{
    public const string Permission = "Permission";
}
