using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.ViewModels.User;

namespace Team_Task_Manager.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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
                var user = await _userService.CreateUser(userViewModel);
                if (user is null) return BadRequest("Can not Create User");
                //Email Service Usage
            }
            return RedirectToAction("LoginBasic", "Auth");
        }
        [HttpPost]
        public async Task<IActionResult> CurrentUser(SignInViewModel signInUser)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.SignInUser(signInUser);
                if (user.Value is null) return BadRequest("User Not Found");

                return RedirectToAction(nameof(Index), "Dashboards");
            }
            return View();
        }
        public async Task<IActionResult> SignOutUser()
        {
            await _userService.LogoutUser();
            return RedirectToAction("Login", "Account");
        }
    }
}
