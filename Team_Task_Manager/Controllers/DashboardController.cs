using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.Services.Interfaces;

namespace Team_Task_Manager.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboard;

        public DashboardController(IDashboardService dashboard)
        {
            _dashboard = dashboard;
        }

        public IActionResult Index()
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var dashboard = _dashboard.GetUserDashboard(userId);
            return View(dashboard);
        }
    }
}
