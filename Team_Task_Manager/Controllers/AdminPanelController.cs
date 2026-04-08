using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.ViewModels.AdminPanel;

namespace Team_Task_Manager.Controllers
{
    public class AdminPanelController : Controller
    {
        private readonly IAdminService _adminService;
        public AdminPanelController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        public IActionResult Index()
        {
            var viewModel = new AdminPanelIndexViewModel
            {
                Roles = _adminService.GetAllRoles().ToList(),
                Permissions = _adminService.GetAllPermissions().ToList(),
            };
            return View(viewModel);
        }
        public IActionResult Permissions()
        {
            var permissions = _adminService.GetAllPermissions();
            return View(permissions);
        }

        public IActionResult Roles()
        {
            var roles = _adminService.GetAllRoles();
            return View(roles);
        }

        public async Task<IActionResult> GetPermissionsForRole()
        {
            var permissions = await _adminService.GetRolePermissions(1);
            return View(permissions);
        }

        [HttpPost]
        public async Task<IActionResult> AssignPermissionsForRole(long SelectedRoleId, List<long> SelectedPermissionIds)
        {
            var selectedPermissions = _adminService.GetAllPermissions()
                .Where(p => SelectedPermissionIds.Contains(p.Id))
                .ToList();
            await _adminService.AssignRolePermissions(SelectedRoleId, selectedPermissions);
            return RedirectToAction(nameof(Index),"Dashboards");
        }
    }
}
