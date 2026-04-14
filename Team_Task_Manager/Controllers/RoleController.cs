using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.Role;
using Team_Task_Manager.Services.Interfaces;

namespace Team_Task_Manager.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<List<Permission>> GetPermissionsForRole()
        {
            return await _roleService.GetRolePermissions(1);
        }
        public ICollection<UserRoles> GetAllUserRoles()
        {
            return _roleService.GetAllRoles();
        }

        [HttpPost]
        public IActionResult Create(string roleName)
        {
            _roleService.CreateRole(roleName);
            return RedirectToAction(nameof(Index), "Dashboard");
        }

        [HttpPost]
        public IActionResult Edit(UserRoles role)
        {
            _roleService.EditRole(role);
            return RedirectToAction(nameof(Index), "Dashboard");

        }
        [HttpPost]
        public IActionResult Delete(long id)
        {
            _roleService.DeleteRole(id);
            return RedirectToAction(nameof(Index), "Dashboard");
        }
    }
}
