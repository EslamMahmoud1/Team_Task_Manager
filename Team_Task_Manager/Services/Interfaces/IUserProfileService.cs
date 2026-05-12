using Team_Task_Manager.Shared;
using Team_Task_Manager.ViewModels.UpdateProfile;

namespace Team_Task_Manager.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileViewModel?> GetProfileAsync(long userId);
        Task<Result<string>> ValidatePersonalInfoAsync(PersonalInfoViewModel vm);
        Task<Result<string>> ValidateEducationsAsync(List<EducationViewModel> vm);
        Task<Result<string>> ValidateSkillsAsync(SkillsViewModel vm);
        Task<Result<string>> SaveFullProfileAsync(long userId, UserProfileViewModel vm);
        Task<Result<string>> SavePersonalInfoAsync(long userId, PersonalInfoViewModel vm);
        Task<Result<string>> SaveEducationsAsync(long userId, List<EducationViewModel> vm);
        Task<Result<string>> SaveSkillsAsync(long userId, SkillsViewModel vm);
        Task<Result<string>> SaveDraftAsync(long userId, PersonalInfoViewModel vm);
    }
}
