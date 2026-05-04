using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Team_Task_Manager.Data;

namespace Team_Task_Manager.Components
{
    public class NavBarViewComponent : ViewComponent
    {
        private readonly TaskAppDbContext _DbContext;

        public NavBarViewComponent(TaskAppDbContext dbContext)
        {
            _DbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdClaim = UserClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return View(new List<string>());
            }
            var userId = long.Parse(userIdClaim.Value);
            var user = await _DbContext.TaskUsers.FirstOrDefaultAsync(u => u.Id == userId);

            var permissionNames = await _DbContext.RolePermissions
                            .Where(rp => rp.RoleId == user.UserRoleId)
                            .Select(rp => rp.Permission.Name.ToString())
                            .ToListAsync();

            return View(permissionNames);
        }
    }
}
