using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Team_Task_Manager.Models.Entities.Role;
using Team_Task_Manager.Services.Implementations;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.ViewModels.Role;

namespace Team_Task_Manager.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public IActionResult Index()
        {
            var roles = _roleService.GetAllRoles();
            var rolesSelectMenu = new RoleIndexViewModel
            {
                Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList()
            };

            rolesSelectMenu.Roles.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "-- Select Role --"
            });
            return View(rolesSelectMenu);
        }
        [HttpPost]
        public async Task<IActionResult> GetPermissionsForRole(long SelectedRoleId)
        {
            var roles = _roleService.GetAllRoles();

            var rolesSelectMenu = new RoleIndexViewModel
            {
                SelectedRoleId = SelectedRoleId,
                Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList()
            };

            rolesSelectMenu.Roles.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "-- Select Role --"
            });

            var permissions = await _roleService.GetRolePermissions(SelectedRoleId);

            rolesSelectMenu.Permissions = permissions.Select(p => p.Name.ToString()).ToList();

            return View("Index", rolesSelectMenu);
        }
        

        [HttpPost]
        public IActionResult Edit(UserRoles role)
        {
            _roleService.EditRole(role);
            return RedirectToAction(nameof(Index), "Dashboard");

        }
        public IActionResult DeleteRoles()
        {
            var roles = _roleService.GetAllRoles();
            var rolesSelectMenu = new RoleIndexViewModel
            {
                Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList()
            };

            rolesSelectMenu.Roles.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "-- Select Role --"
            });
            return View(rolesSelectMenu);
        }
        public async Task<IActionResult> Delete(long SelectedRoleId)
        {
            var roles = _roleService.GetAllRoles();
            var role = roles.FirstOrDefault(r => r.Id == SelectedRoleId);
            return View(role);
        }
        [HttpPost]
        public IActionResult DeleteConfirmed(long Id)
        {
            
            try
            {
                _roleService.DeleteRole(Id);
                return RedirectToAction(nameof(Index), "Role");

            }
            catch (Exception ex)
            {

                throw new Exception("An error occurred while deleting the role.", ex);
            }
        }
    }
}
