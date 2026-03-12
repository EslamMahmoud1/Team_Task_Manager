namespace Team_Task_Manager.ViewModels.Task
{
    public class TaskViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } 
        public DateTime DueDate { get; set; }
        public string? Status { get; set; } 
        public string? Priority { get; set; }
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
