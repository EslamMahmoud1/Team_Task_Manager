using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Team_Task_Manager.Extesions;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.ViewModels.User;
using Team_Task_Manager.ViewModels.Users;

namespace Team_Task_Manager.Controllers
{
    [Authorize]
    [AuthFilter(ClaimName.Permission, PermissionName.AdminPanel)]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IRoleService _roleService;

        public UsersController(IUsersService usersService, IRoleService roleService)
        {
            _usersService = usersService;
            _roleService = roleService;
        }


        // GET: UsersController
        public async Task<ActionResult> Index()
        {
            var users = await _usersService.GetAllAsync();
            return View(users);
        }

        // GET: UsersController/Details/5
        public async Task<ActionResult> Details(long id)
        {
            var userResult = await _usersService.GetByIdAsync(id);
            var detailedUser = userResult.Value.Adapt<UserDetailsViewModel>();
            
            return View(detailedUser);
        }

        // GET: UsersController/Create
        public ActionResult Create()
        {
            return RedirectToAction("RegisterBasic", "Auth");
        }


        // GET: UsersController/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            var user = await _usersService.GetByIdAsync(id);
            var rolesList = await _roleService.GetAllRolesAsync();
            var roles = rolesList.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();

            ViewBag.UserRoles = roles;
            return View(user.Value);
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(UserEditViewModel userEdit)
        {
            var result = await _usersService.UpdateAsync(userEdit);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.UserRoles = await _roleService.GetAllRolesAsync();
            return View(userEdit);
        }

        // GET: UsersController/Delete/5
        public async Task<ActionResult> Delete(long id)
        {
            var user = await _usersService.GetByIdAsync(id);
            return View(user.Value);
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            var result = await _usersService.DeleteAsync(id);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
