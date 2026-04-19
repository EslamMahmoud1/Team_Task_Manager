using Microsoft.AspNetCore.Mvc;

namespace Team_Task_Manager.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
