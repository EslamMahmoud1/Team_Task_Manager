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

        public IActionResult Index(TaskUser user)
        {
            var dashboard = _dashboard.GetUserDashboard(user);
            return View(dashboard);
        }
    }
}
