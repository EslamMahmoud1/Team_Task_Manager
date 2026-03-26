using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Text.RegularExpressions;
using Team_Task_Manager.Data;
using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.ViewModels.User;

namespace Team_Task_Manager.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly TaskAppDbContext _context;

        public UserService(TaskAppDbContext context)
        {
            _context = context;
        }

        public async Task<TaskUser> CreateUser(UserViewModel userViewModel)
        {
            if(userViewModel is null) throw new Exception("UserViewModel cannot be null");
            if(_context.Users.Any(u => u.Email == userViewModel.Email)) throw new Exception("User with this email already exists");

            var emailRegex = new Regex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");
            if(!emailRegex.IsMatch(userViewModel.Email)) throw new Exception("Invalid email format");

            var user = new TaskUser() { Email = userViewModel.Email, Name = userViewModel.Name };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<TaskUser> SignInUser(string Email)
        {
            var user = await _context.Users.Include(u => u.CreatedTasks).Include(u => u.AssignedTasks).FirstOrDefaultAsync(u => u.Email == Email);
            if(user == null) throw new Exception("User with this email does not exist");
            

            return user;
        }
    }
}
