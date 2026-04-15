using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Team_Task_Manager.Models.Entities.Permissions;

namespace Team_Task_Manager.Extesions;

public class AuthFilterAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _claimType;
    private readonly PermissionName _claimValue;

    public AuthFilterAttribute(string claimType, PermissionName claimValue)
    {
        _claimType = claimType;
        _claimValue = claimValue;
    }
    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }
        var hasClaim = context.HttpContext.User.Claims.Any(c =>
            c.Type == _claimType && c.Value == _claimValue.ToString());

        if (!hasClaim)
        {
            context.HttpContext.Response.Cookies.Delete("TaskAppAuthCookie");
            await context.HttpContext.SignOutAsync();
            //context.Result = new ForbidResult();
            context.Result = new RedirectToActionResult("LoginBasic", "Auth", null);
        }
    }
}

public static class ClaimName
{
    public const string Permission = "Permission";
}
