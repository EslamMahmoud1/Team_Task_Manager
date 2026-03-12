using Microsoft.AspNetCore.Mvc;

namespace Team_Task_Manager.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
