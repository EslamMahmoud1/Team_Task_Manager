using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Extesions;
using Team_Task_Manager.Models.Entities.Permissions;

namespace AspnetCoreMvcFull.Controllers;

public class LayoutExamplesController : Controller
{
    [AuthFilter(ClaimName.Permission, PermissionName.Blank)]
    public IActionResult Blank() => View();

    [AuthFilter(ClaimName.Permission, PermissionName.Container)]
    public IActionResult Container() => View();

    [AuthFilter(ClaimName.Permission, PermissionName.Fluid)]
    public IActionResult Fluid() => View();

    public IActionResult HorizontalMenu() => View();


    public IActionResult WithoutMenu() => View();

    [AuthFilter(ClaimName.Permission, PermissionName.Withoutnavbar)]
    public IActionResult WithoutNavbar() => View();
}
