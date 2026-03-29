using Team_Task_Manager.Models.Entities.Task;
using Team_Task_Manager.ViewModels.Task;
using Team_Task_Manager.ViewModels.User;

namespace Team_Task_Manager.Services.Interfaces
{
    public interface ITaskService
    {
        public Task<TaskItem> CreateTask(TaskViewModel taskViewModel, long creatorId);
        public Task<IEnumerable<SelectUserList>> GetUsers();
        public Task<TaskItem> GetTaskById(long id);
        public Task<int> CompeleteTask(long taskId);
        public Task<int> UnCompeleteTask(long taskId);
    }
}
