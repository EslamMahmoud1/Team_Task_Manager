using Team_Task_Manager.Models.Entities.Task;

namespace Team_Task_Manager.Models.Entities.User
{
    public class TaskUser
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
        public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
    }
}
