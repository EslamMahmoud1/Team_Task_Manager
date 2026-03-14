using Mapster;
using Team_Task_Manager.Models.Entities.Task;
using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.ViewModels.Dashboard;
using Team_Task_Manager.ViewModels.Task;

namespace Team_Task_Manager.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        public DashboardViewModel GetUserDashboard(TaskUser user)
        {
            var createdTasks = user.CreatedTasks.Adapt<List<TaskViewModel>>();
            var assignedTasks = user.AssignedTasks.Adapt<List<TaskViewModel>>();

            var dashboard = new DashboardViewModel()
            {
                UserName = user.Name,
                CreatedTasks = createdTasks,
                AssignedTasks = assignedTasks,
                CompletedTasks = user.CreatedTasks.Count(t => t.Status == TaskStat.Completed) + user.AssignedTasks.Count(t => t.Status == TaskStat.Completed),
                PendingTasks = user.CreatedTasks.Count(t => t.Status != TaskStat.Completed) + user.AssignedTasks.Count(t => t.Status != TaskStat.Completed),
                TotalTasksCreated = user.CreatedTasks.Count,
                TotalTasksAssigned = user.AssignedTasks.Count
            };
            return dashboard;
        }
    }
}
