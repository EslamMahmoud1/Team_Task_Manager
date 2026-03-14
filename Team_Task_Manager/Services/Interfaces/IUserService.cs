using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.ViewModels.User;

namespace Team_Task_Manager.Services.Interfaces
{
    public interface IUserService
    {
        public Task<TaskUser> CreateUser(UserViewModel userViewModel);
        public Task<TaskUser> SignInUser(string Email);
    }
}
