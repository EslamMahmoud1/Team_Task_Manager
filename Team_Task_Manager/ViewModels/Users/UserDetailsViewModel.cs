namespace Team_Task_Manager.ViewModels.Users
{
    public class UserDetailsViewModel
    {
        public long Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string UserName { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
        public int AccessFailedCount { get; init; }
        public string UserRole { get; init; } = string.Empty;
    }
}
