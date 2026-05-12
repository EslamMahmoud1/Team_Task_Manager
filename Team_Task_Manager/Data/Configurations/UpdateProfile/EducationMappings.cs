using Team_Task_Manager.Models.Entities.UpdateProfile;
using Team_Task_Manager.ViewModels.UpdateProfile;

namespace Team_Task_Manager.Data.Configurations.UpdateProfile
{
    public static class EducationMappings
    {
        public static void ApplyFrom(this Education entity, EducationViewModel vm)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (vm == null) return;

            entity.Institution = vm.Institution?.Trim();
            entity.Degree = vm.Degree?.Trim();
            entity.FieldOfStudy = vm.FieldOfStudy?.Trim();
            entity.GraduationYear = vm.GraduationYear;
            entity.Description = vm.Description;
        }
    }
}
