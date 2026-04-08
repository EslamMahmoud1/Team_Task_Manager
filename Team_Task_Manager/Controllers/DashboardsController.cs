using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers;

public class DashboardsController : Controller
{
    public IActionResult Index() => View();
    public IActionResult CRM() => View();
    public IActionResult eCommerce() => View();
    public IActionResult Logistics() => View();
    public IActionResult Academy() => View();
}
