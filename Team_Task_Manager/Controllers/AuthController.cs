using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Extesions;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Services.Interfaces;

namespace AspnetCoreMvcFull.Controllers;

public class AuthController : Controller
{
    private readonly IRoleService _roleService;

    public AuthController(IRoleService roleService)
    {
        _roleService = roleService;
    }
    [AuthFilter(ClaimName.Permission, PermissionName.ForgotPassword)]
    public IActionResult ForgotPasswordBasic() => View();
    public IActionResult LoginBasic() => View();

    public IActionResult RegisterBasic()
    {
        ViewBag.Roles = _roleService.GetAllRoles();
        return View();
    }
    public IActionResult AccessDenied() => View();
}
