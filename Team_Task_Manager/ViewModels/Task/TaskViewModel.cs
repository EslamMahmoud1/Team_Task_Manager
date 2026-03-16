using Microsoft.AspNetCore.Mvc.Rendering;
using Team_Task_Manager.Models.Entities.Task;

namespace Team_Task_Manager.ViewModels.Task
{
    public class TaskViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } 
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public long AssignedToId { get; set; }
        public IEnumerable<SelectListItem>? Users { get; set; }

    }
}
