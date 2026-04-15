using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.ViewModels.Task;

namespace Team_Task_Manager.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET: TaskController/Create
        public async Task<ActionResult> Create()
        {
            var users = await _taskService.GetUsers();
            var flag = HttpContext.Request.Cookies.TryGetValue("UserId", out var userIdStr);
            var userId = flag ? userIdStr : "";
            
            var filteredUsers = users.Where(u => u.Id != userId).ToList();

            ViewBag.AssignedToId = new SelectList(filteredUsers, "Id", "Name");
            return View();
        }

        // POST: TaskController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TaskViewModel taskViewModel)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var task = await _taskService.CreateTask(taskViewModel, userId);
            return RedirectToAction(nameof(Index), "Dashboard");
        }

        public async Task<ActionResult> Details(int id)
        {
            var task = await _taskService.GetTaskById(id);
            var mappedTask = task.Adapt<ShowTaskViewModel>();
            return View(mappedTask);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(long id)
        {
            await _taskService.CompeleteTask(id);
            return RedirectToAction(nameof(Index), "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnComplete(long id)
        {
            await _taskService.UnCompeleteTask(id);
            return RedirectToAction(nameof(Index), "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            await _taskService.DeleteTask(id);
            return RedirectToAction(nameof(Index), "Dashboard");
        }
    }
}
