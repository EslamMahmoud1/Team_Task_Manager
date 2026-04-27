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
            if (!ModelState.IsValid) return BadRequest("False Credentials");

            var user = await _userService.SignInUser(signInUser);
            if (user.Value is null) return BadRequest(user.Errors);

            return RedirectToAction(nameof(Index), "Dashboards");
        }
        public async Task<IActionResult> SignOutUser()
        {
            await _userService.LogoutUser();
            return RedirectToAction("Login", "Account");
        }
    }
}
