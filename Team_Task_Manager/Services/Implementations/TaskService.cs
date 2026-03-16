using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Team_Task_Manager.Data;
using Team_Task_Manager.Models.Entities.Task;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.ViewModels.Task;
using Team_Task_Manager.ViewModels.User;

namespace Team_Task_Manager.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly TaskAppDbContext _context;

        public TaskService(TaskAppDbContext context)
        {
            _context = context;
        }

        public async Task<TaskItem> CreateTask(TaskViewModel model, long creatorId)
        {
            var taskItem = new TaskItem
            {
                Title = model.Title,
                Description = model.Description ?? string.Empty,
                DueDate = model.DueDate,
                Priority = model.Priority,
                AssignedToId = model.AssignedToId,
                CreatedById = creatorId,
                Status = TaskStat.InProgress
            };
            await _context.Tasks.AddAsync(taskItem);
            await _context.SaveChangesAsync();
            return taskItem;
        }

        public async Task<IEnumerable<SelectUserList>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            var userList = users.Select(u => new SelectUserList
            {
                Id = u.Id.ToString(),
                Name = u.Name
            });
            return userList;
        }
    }
}
