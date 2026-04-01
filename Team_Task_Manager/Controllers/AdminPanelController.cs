using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Services.Interfaces;

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
            return View();
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
        public async Task<IActionResult> AssignPermissionsForRole()
        {
            await _adminService.AssignRolePermissions(1, new List<Permission>
            {
                new Permission { Name = PermissionName.CreateTask },
                new Permission { Name = PermissionName.ViewTask },
                new Permission { Name = PermissionName.ChangeStatus},
                new Permission { Name = PermissionName.DeleteTask},

            });
            return RedirectToAction(nameof(Index));
        }
    }
}
