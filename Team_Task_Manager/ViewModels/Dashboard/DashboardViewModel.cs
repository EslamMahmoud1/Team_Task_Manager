using Team_Task_Manager.ViewModels.Task;

namespace Team_Task_Manager.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public string UserName { get; set; }

        public int TotalTasksCreated { get; set; }
        public int TotalTasksAssigned { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }

        public List<TaskViewModel> CreatedTasks { get; set; }
        public List<TaskViewModel> AssignedTasks { get; set; }
    }
}
