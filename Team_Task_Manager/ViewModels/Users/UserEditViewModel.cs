namespace Team_Task_Manager.ViewModels.Users
{
    public class UserEditViewModel
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public long UserRoleId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
