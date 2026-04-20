using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Extesions;
using Team_Task_Manager.Models.Entities.Permissions;

namespace AspnetCoreMvcFull.Controllers;

public class DashboardsController : Controller
{
    public IActionResult Index() => View();

    [AuthFilter(ClaimName.Permission, PermissionName.CRM)]
    public IActionResult CRM() => View();

    [AuthFilter(ClaimName.Permission, PermissionName.eCommerce)]
    public IActionResult eCommerce() => View();

    [AuthFilter(ClaimName.Permission, PermissionName.Logistics)]
    public IActionResult Logistics() => View();

    [AuthFilter(ClaimName.Permission, PermissionName.Academy)]
    public IActionResult Academy() => View();
}
