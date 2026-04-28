using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Extesions;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.ViewModels.AdminPanel;

namespace Team_Task_Manager.Controllers
{
    [Authorize]
    public class AdminPanelController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IRoleService _roleService;
        public AdminPanelController(IAdminService adminService, IRoleService roleService)
        {
            _adminService = adminService;
            _roleService = roleService;
        }
        [AuthFilter(ClaimName.Permission, PermissionName.AdminPanel)]
        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminPanelIndexViewModel
            {
                Roles = await _roleService.GetAllRolesAsync(),
                Permissions = _adminService.GetAllPermissions().ToList(),
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignPermissionsForRole(string RoleName, List<long> SelectedPermissionIds)
        {
            if (SelectedPermissionIds == null || !SelectedPermissionIds.Any())
                return BadRequest("No permissions selected");

            var selectedPermissions = _adminService.GetAllPermissionsByIds(SelectedPermissionIds);

            var role = await _roleService.CreateRole(RoleName);
            if (!role.IsSuccess) return Conflict(role.Errors);

            await _adminService.AssignRolePermissions(RoleName, selectedPermissions);
            return RedirectToAction("Index","Role");
        }
    }
}
