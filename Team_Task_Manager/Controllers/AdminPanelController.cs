using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Extesions;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.ViewModels.AdminPanel;

namespace Team_Task_Manager.Controllers
{
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
        public IActionResult Index()
        {
            var viewModel = new AdminPanelIndexViewModel
            {
                Roles = _roleService.GetAllRoles().ToList(),
                Permissions = _adminService.GetAllPermissions().ToList(),
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignPermissionsForRole(string RoleName, List<long> SelectedPermissionIds)
        {
            var selectedPermissions = _adminService.GetAllPermissions()
                .Where(p => SelectedPermissionIds.Contains(p.Id))
                .ToList();

            var roleId = await _roleService.CreateRole(RoleName);
            if (roleId.Value == 0) return BadRequest("Role Already Exists");

            await _adminService.AssignRolePermissions(RoleName, selectedPermissions);
            return RedirectToAction(nameof(Index),"Dashboards");
        }
    }
}
