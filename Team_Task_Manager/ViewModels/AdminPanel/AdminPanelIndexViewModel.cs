using System.Collections.Generic;
using Team_Task_Manager.Models.Entities.Permissions;
using Team_Task_Manager.Models.Entities.Role;

namespace Team_Task_Manager.ViewModels.AdminPanel
{
    public class AdminPanelIndexViewModel
    {
        public List<UserRoles> Roles { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
