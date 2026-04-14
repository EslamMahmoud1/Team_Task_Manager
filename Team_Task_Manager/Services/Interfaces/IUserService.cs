using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.Shared;
using Team_Task_Manager.ViewModels.User;

namespace Team_Task_Manager.Services.Interfaces
{
    public interface IUserService
    {
        public Task<Result<TaskUser>> CreateUser(UserViewModel userViewModel);
        public Task<Result<TaskUser>> SignInUser(SignInViewModel user);
        public Task LogoutUser();
    }
}
