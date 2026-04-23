using System.ComponentModel.DataAnnotations;

namespace Team_Task_Manager.ViewModels.Users
{
    public class UserCreateViewModel 
    {
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public long RoleId { get; set; }
    }

}
