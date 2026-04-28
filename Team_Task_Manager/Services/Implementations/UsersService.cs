using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.Shared;
using Team_Task_Manager.ViewModels.Users;

namespace Team_Task_Manager.Services.Implementations
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<TaskUser> _userManager;
        private readonly SignInManager<TaskUser> _signInManager;


        public UsersService(UserManager<TaskUser> userManager, SignInManager<TaskUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IEnumerable<UserDetailsViewModel>> GetAllAsync()
        {
            var users = await _userManager.Users.Include(u => u.UserRole).ToListAsync();

            return users.Select(u => new UserDetailsViewModel
            {
                Id = u.Id,
                Email = u.Email ?? "",
                UserName = u.UserName ?? "",
                UserRole = u.UserRole.Name ?? "",
                PhoneNumber = u.PhoneNumber,
                AccessFailedCount = u.AccessFailedCount
            });
        }

        public async Task<Result<TaskUser>> GetByIdAsync(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return Result<TaskUser>.Failure(new List<string> { "User not found" });
            return Result<TaskUser>.Success(user);
        }



        public async Task<bool> UpdateAsync(UserEditViewModel userEdit)
        {
            var userResult = await GetByIdAsync(userEdit.Id);
            if (!userResult.IsSuccess) return false;

            var user = userResult.Value;

            user.Email = userEdit.Email;
            user.UserName = userEdit.UserName;
            user.UserRoleId = userEdit.UserRoleId;
            user.PhoneNumber = userEdit.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            //await _userManager.UpdateSecurityStampAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var userResult = await GetByIdAsync(id);
            if (!userResult.IsSuccess) return false;

            var user = userResult.Value;

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded;
        }
    }
}
