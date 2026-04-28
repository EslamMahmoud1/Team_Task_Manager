using Team_Task_Manager.Models.Entities.Permissions;

namespace Team_Task_Manager.ViewModels.Role
{
    public class RoleDetailsViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
