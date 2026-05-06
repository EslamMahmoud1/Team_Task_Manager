using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.ViewModels.Role;

namespace Team_Task_Manager.Controllers
{
    [PermissionAuthFilter(PermissionName.AdminPanel)]

    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return View(roles);
        }
        [HttpPost]
        public IActionResult Create()
        {
            return RedirectToAction("Index", "AdminPanel");
        }
        public async Task<IActionResult> Details(long id)
        {
            var role = await _roleService.GetRoleById(id);
            var permissions = await _roleService.GetRolePermissions(id);
            var roleDetails = new RoleDetailsViewModel
            {
                Id = id,
                Name = role.Name ?? "",
                Permissions = permissions
            };
            return View(roleDetails);
        }

        public async Task<ActionResult> Edit(long id)
        {
            var model = await _roleService.GetRoleById(id);
            var permissions = await _roleService.GetRolePermissions(id);
            ViewBag.selectedPermissions = permissions.Select(p => p.Id).ToList();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string RoleName, List<long> SelectedPermissionIds)
        {
            var result = await _roleService.EditRole(RoleName, SelectedPermissionIds);
            if (!result.IsSuccess) return Conflict(result.Errors);
            return Json(new { success = true });
        }
        public async Task<IActionResult> Delete(long id)
        {
            var role = await _roleService.GetRoleById(id);
            return View(role);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var result = await _roleService.DeleteRole(id);
            if (!result.IsSuccess) return Conflict(result.Errors);
            return RedirectToAction(nameof(Index), "Role");
        }
        public async Task<IActionResult> HasUsers(long roleId)
        {
            var role = await _roleService.GetRoleById(roleId);
            if (role == null) return Json(new { hasUsers = false, count = 0 });

            var users = await _roleService.HasUsers(roleId);

            return Json(new
            {
                hasUsers = users.Any(),
                count = users.Count
            });
        }

        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Json(roles);
        }

        [HttpPost]
        public async Task<IActionResult> ReassignUsersAndDelete(long roleId, long newRoleId)
        {
            var result = await _roleService.ReassignUsersAndDelete(roleId, newRoleId);
            if (!result.IsSuccess) return Conflict(result.Errors);
            return Json(new { success = true });

        }
    }
}
