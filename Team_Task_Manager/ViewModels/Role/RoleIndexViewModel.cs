using Microsoft.AspNetCore.Mvc.Rendering;

namespace Team_Task_Manager.ViewModels.Role;

public class RoleIndexViewModel
{
    public long SelectedRoleId { get; set; }

    public List<SelectListItem> Roles { get; set; } = new();

    public List<string> Permissions { get; set; } = new();

    public bool ShowPermissions => SelectedRoleId != 0 && Permissions.Any();
}
