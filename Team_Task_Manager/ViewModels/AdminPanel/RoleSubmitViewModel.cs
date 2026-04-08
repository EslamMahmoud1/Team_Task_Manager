namespace Team_Task_Manager.ViewModels.AdminPanel
{
    public class RoleSubmitViewModel
    {
        public long SelectedRoleId { get; set; }
        public List<long> SelectedPermissionIds { get; set; } = new List<long>();
    }
}
