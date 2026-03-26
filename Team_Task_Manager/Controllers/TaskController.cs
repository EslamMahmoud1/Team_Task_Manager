using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            ViewBag.AssignedToId = new SelectList(users, "Id", "Name");
            return View();
        }

        // POST: TaskController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TaskViewModel taskViewModel)
        {
            var flag = HttpContext.Request.Cookies.TryGetValue("UserId", out var userIdStr);
            var userId = flag ? long.Parse(userIdStr) : 0;

            var task = await _taskService.CreateTask(taskViewModel, userId);
            return RedirectToAction(nameof(Index), "Dashboard");
        }

        public ActionResult Details(int id)
        {
            var task = _taskService.GetTaskById(id);
            return View(task);
        }

        // POST: TaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TaskController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TaskController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
