using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Extesions;
using Team_Task_Manager.Models.Entities.Permissions;

namespace AspnetCoreMvcFull.Controllers;

public class DashboardsController : Controller
{
    public IActionResult Index() => View();
    [AuthFilter(ClaimName.Permission, PermissionName.CRM)]
    public IActionResult CRM() => View();
    public IActionResult eCommerce() => View();
    public IActionResult Logistics() => View();
    public IActionResult Academy() => View();
}
