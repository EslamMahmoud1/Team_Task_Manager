using Mapster;
using Microsoft.EntityFrameworkCore;
using Team_Task_Manager.Data;
using Team_Task_Manager.Models.Entities.Task;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.ViewModels.Dashboard;
using Team_Task_Manager.ViewModels.Task;

namespace Team_Task_Manager.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly TaskAppDbContext _context;

        public DashboardService(TaskAppDbContext context)
        {
            _context = context;
        }

        public DashboardViewModel GetUserDashboard(long userId)
        {
            var user = _context.Users.Include(u => u.CreatedTasks).ThenInclude(t => t.AssignedTo)
                .Include(u => u.AssignedTasks).ThenInclude(t => t.CreatedBy)
                .FirstOrDefault(u => u.Id == userId);

            var createdTasks = user?.CreatedTasks.Adapt<List<ShowTaskViewModel>>() ?? new List<ShowTaskViewModel>();
            var assignedTasks = user?.AssignedTasks.Adapt<List<ShowTaskViewModel>>() ?? new List<ShowTaskViewModel>();

            var dashboard = new DashboardViewModel()
            {
                UserName = user?.UserName ?? "",
                CreatedTasks = createdTasks ,
                AssignedTasks = assignedTasks,
                CompletedTasks = user.AssignedTasks.Count(t => t.Status == TaskStat.Completed),
                PendingTasks = user.AssignedTasks.Count(t => t.Status != TaskStat.Completed),
                TotalTasksCreated = user.CreatedTasks.Count,
                TotalTasksAssigned = user.AssignedTasks.Count
            };
            return dashboard;
        }
    }
}
