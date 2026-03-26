namespace Team_Task_Manager.ViewModels.Task
{
    public class ShowTaskViewModel
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public DateTime DueDate { get; set; }
        public string? AssignedToName { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
