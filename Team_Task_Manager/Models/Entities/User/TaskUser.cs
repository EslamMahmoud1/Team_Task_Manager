using Microsoft.AspNetCore.Identity;
using Team_Task_Manager.Models.Entities.Role;
using Team_Task_Manager.Models.Entities.Task;
using Team_Task_Manager.Models.Entities.UpdateProfile;

namespace Team_Task_Manager.Models.Entities.User
{
    public class TaskUser : IdentityUser<long>
    {
        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
        public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
        public UserProfile Profile { get; set; }
        public UserRoles UserRole { get; set; }
        public long UserRoleId { get; set; }
    }
}
