using Team_Task_Manager.Models.Entities.User;

namespace Team_Task_Manager.Models.Entities.Task
{
    public class TaskItem
    {
        public long Id { get; set; }
        public required string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public required DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public TaskStat Status { get; set; } = TaskStat.InProgress;

        public TaskUser AssignedTo { get; set; } = null!;
        public long AssignedToId { get; set; } 

        public TaskUser CreatedBy { get; set; } = null!;
        public long CreatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
