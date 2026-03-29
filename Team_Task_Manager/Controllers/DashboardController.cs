using Microsoft.AspNetCore.Mvc;
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
            var flag = HttpContext.Request.Cookies.TryGetValue("UserId", out var userIdStr);
            var userId = flag ? long.Parse(userIdStr!) : 0;

            var dashboard = _dashboard.GetUserDashboard(userId);
            return View(dashboard);
        }
    }
}
