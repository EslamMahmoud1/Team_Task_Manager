namespace Team_Task_Manager.ViewModels.Role
{
    public class RoleEditViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<long> SelectedPermissionIds { get; set; } = new List<long>();
    }
}
