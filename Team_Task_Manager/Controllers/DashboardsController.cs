using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Extesions;
using Team_Task_Manager.Models.Entities.Permissions;

namespace AspnetCoreMvcFull.Controllers;


public class DashboardsController : Controller
{
    [PermissionAuthFilter(PermissionName.CRM)]
    public IActionResult Index() => View();
    [PermissionAuthFilter(PermissionName.CRM)]
    public IActionResult CRM() => View();
    [PermissionAuthFilter(PermissionName.eCommerce)]
    public IActionResult eCommerce() => View();

    [PermissionAuthFilter(PermissionName.Logistics)]
    public IActionResult Logistics() => View();
    [PermissionAuthFilter(PermissionName.Academy)]
    public IActionResult Academy() => View();
}
