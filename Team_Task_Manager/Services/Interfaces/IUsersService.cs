using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.Shared;
using Team_Task_Manager.ViewModels.Users;

namespace Team_Task_Manager.Services.Interfaces
{
    public interface IUsersService
    {
        Task<IEnumerable<UserDetailsViewModel>> GetAllAsync();
        Task<Result<TaskUser>> GetByIdAsync(long id);
        Task<bool> UpdateAsync(UserEditViewModel dto);
        Task<bool> DeleteAsync(long id);
    }
}
