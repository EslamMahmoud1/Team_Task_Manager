using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.ViewModels.Dashboard;

namespace Team_Task_Manager.Services.Interfaces
{
    public interface IDashboardService
    {
        public DashboardViewModel GetUserDashboard(long userId);
    }
}
