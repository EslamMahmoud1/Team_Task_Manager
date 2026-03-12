using Mapster;
using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Data;
using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.ViewModels.User;

namespace Team_Task_Manager.Controllers
{
    public class UserController : Controller
    {
        private readonly TaskAppDbContext _context;

        public UserController(TaskAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateUser()
        {
            return View();
        }

        public IActionResult CurrentUser()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateUser(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = userViewModel.Adapt<TaskUser>();
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Dashboard");
            }
            return View();
        }
        [HttpPost]
        public IActionResult CurrentUser(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index), "Dashboard");
            }
            return View();
        }
    }
}
