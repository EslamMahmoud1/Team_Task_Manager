using Microsoft.EntityFrameworkCore;
using Team_Task_Manager.Data;
using Team_Task_Manager.Data.Configurations.UpdateProfile;
using Team_Task_Manager.Models.Entities.UpdateProfile;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.Shared;
using Team_Task_Manager.ViewModels.UpdateProfile;

namespace Team_Task_Manager.Services.Implementations
{
    public class UserProfileService : IUserProfileService
    {
        private readonly TaskAppDbContext _db;

        public UserProfileService(TaskAppDbContext db)
        {
            _db = db;
        }

        // ── GET ─────────────────────────────────────────────────
        public async Task<UserProfileViewModel?> GetProfileAsync(long userId)
        {
            var profile = await _db.UserProfiles
                .Include(p => p.User)
                .Include(p => p.Educations)
                .Include(p => p.Skills).ThenInclude(s => s.Skill)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return profile?.ToViewModel();
        }

        // ── Validation-only endpoints used by section AJAX ───────
        public Task<Result<string>> ValidatePersonalInfoAsync(PersonalInfoViewModel vm)
        {
            var errors = ValidatePersonalInfo(vm);
            return Task.FromResult(errors.Count > 0
                ? Result<string>.Failure(errors.Values.ToList())
                : Result<string>.Success("Personal information is valid."));
        }

        public Task<Result<string>> ValidateEducationsAsync(List<EducationViewModel> vms)
        {
            var errors = ValidateEducations(vms);
            return Task.FromResult(errors.Count > 0
                ? Result<string>.Failure(errors.Values.ToList())
                : Result<string>.Success("Education is valid."));
        }

        public Task<Result<string>> ValidateSkillsAsync(SkillsViewModel vm)
        {
            var errors = ValidateSkills(vm);
            return Task.FromResult(errors.Count > 0
                ? Result<string>.Failure(errors.Values.ToList())
                : Result<string>.Success("Skills are valid."));
        }

        // ── Final submit: the only path that persists profile data ─
        public async Task<Result<string>> SaveFullProfileAsync(long userId, UserProfileViewModel vm)
        {
            if (vm == null)
                return Result<string>.Failure(new List<string> { "Profile payload is required." });

            var errors = ValidateFullProfile(vm);
            if (errors.Count > 0)
                return Result<string>.Failure(errors.Values.ToList());

            await using var transaction = await _db.Database.BeginTransactionAsync();

            var profile = await _db.UserProfiles
                .Include(p => p.Educations)
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                var now = DateTime.UtcNow;
                profile = new UserProfile
                {
                    UserId = userId,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                _db.UserProfiles.Add(profile);
            }

            profile.ApplyFrom(vm.PersonalInfo);
            if (!string.IsNullOrWhiteSpace(vm.PersonalInfo.ProfilePictureUrl))
                profile.ProfilePictureUrl = vm.PersonalInfo.ProfilePictureUrl;

            _db.Educations.RemoveRange(profile.Educations.ToList());
            foreach (var education in vm.Educations)
            {
                _db.Educations.Add(new Education
                {
                    UserProfile = profile,
                    Institution = education.Institution.Trim(),
                    Degree = education.Degree.Trim(),
                    FieldOfStudy = education.FieldOfStudy.Trim(),
                    GraduationYear = education.GraduationYear,
                    Description = education.Description
                });
            }

            _db.UserSkills.RemoveRange(profile.Skills.ToList());

            var skillEntries = NormalizeSkillEntries(vm.Skills);
            var skillTypes = skillEntries
                .Select(entry => Enum.Parse<SkillType>(entry.Name, ignoreCase: true))
                .Distinct()
                .ToList();

            var existingSkills = await _db.Skills
                .Where(skill => skillTypes.Contains(skill.Name))
                .ToListAsync();

            foreach (var entry in skillEntries)
            {
                var skillType = Enum.Parse<SkillType>(entry.Name, ignoreCase: true);
                var skill = existingSkills.FirstOrDefault(existing => existing.Name == skillType);
                if (skill == null)
                {
                    skill = new Skill { Name = skillType };
                    _db.Skills.Add(skill);
                    existingSkills.Add(skill);
                }

                _db.UserSkills.Add(new UserSkill
                {
                    UserProfile = profile,
                    Skill = skill,
                    ProficiencyLevel = entry.ProficiencyLevel!,
                    YearsOfExperience = entry.YearsOfExperience,
                    AdditionalNotes = entry.AdditionalNotes,
                });
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<string>.Success("Profile saved successfully.");
        }

        // ── Section 1: Personal Info ─────────────────────────────
        public Task<Result<string>> SavePersonalInfoAsync(long userId, PersonalInfoViewModel vm)
            => ValidatePersonalInfoAsync(vm);

        // ── Section 2: Education ─────────────────────────────────
        public Task<Result<string>> SaveEducationsAsync(long userId, List<EducationViewModel> vms)
            => ValidateEducationsAsync(vms);

        // ── Section 3: Skills ────────────────────────────────────
        public Task<Result<string>> SaveSkillsAsync(long userId, SkillsViewModel vm)
            => ValidateSkillsAsync(vm);

        // ── Save Draft (no validation) ───────────────────────────
        public Task<Result<string>> SaveDraftAsync(long userId, PersonalInfoViewModel vm)
            => Task.FromResult(Result<string>.Success("Draft is kept locally until final submit."));

        // ════════════════════════════════════════════════════════
        //  Private: Validation
        // ════════════════════════════════════════════════════════

        private static Dictionary<string, string> ValidateFullProfile(UserProfileViewModel vm)
        {
            var errors = new Dictionary<string, string>();

            foreach (var error in ValidatePersonalInfo(vm.PersonalInfo))
                errors[$"PersonalInfo.{error.Key}"] = error.Value;

            foreach (var error in ValidateEducations(vm.Educations))
                errors[error.Key] = error.Value;

            foreach (var error in ValidateSkills(vm.Skills))
                errors[$"Skills.{error.Key}"] = error.Value;

            return errors;
        }

        private static Dictionary<string, string> ValidatePersonalInfo(PersonalInfoViewModel vm)
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(vm.FirstName))
                errors["FirstName"] = "First name is required.";
            else if (vm.FirstName.Length > 50)
                errors["FirstName"] = "Max 50 characters.";

            if (string.IsNullOrWhiteSpace(vm.LastName))
                errors["LastName"] = "Last name is required.";
            else if (vm.LastName.Length > 50)
                errors["LastName"] = "Max 50 characters.";

            if (string.IsNullOrWhiteSpace(vm.Phone))
                errors["Phone"] = "Phone number is required.";
            else if (!string.IsNullOrWhiteSpace(vm.Phone) &&
                !System.Text.RegularExpressions.Regex.IsMatch(vm.Phone, @"^\+?[\d\s\-().]{7,20}$"))
                errors["Phone"] = "Enter a valid phone number.";

            if (vm.DateOfBirth.HasValue &&
                vm.DateOfBirth.Value > DateOnly.FromDateTime(DateTime.Today.AddYears(-13)))
                errors["DateOfBirth"] = "Must be 13 or older.";

            if (!string.IsNullOrWhiteSpace(vm.Headline) && vm.Headline.Length > 120)
                errors["Headline"] = "Max 120 characters.";

            if (!string.IsNullOrWhiteSpace(vm.Bio) && vm.Bio.Length > 1000)
                errors["Bio"] = "Max 1000 characters.";

            return errors;
        }

        private static Dictionary<string, string> ValidateEducations(List<EducationViewModel> vms)
        {
            var errors = new Dictionary<string, string>();

            if (vms == null || vms.Count == 0)
            {
                errors["general"] = "Please add at least one education entry.";
                return errors;
            }

            for (int i = 0; i < vms.Count; i++)
            {
                var e = vms[i];
                if (string.IsNullOrWhiteSpace(e.Institution))
                    errors[$"Educations[{i}].Institution"] = "Institution name is required.";
                if (string.IsNullOrWhiteSpace(e.Degree))
                    errors[$"Educations[{i}].Degree"] = "Degree is required.";
                if (string.IsNullOrWhiteSpace(e.FieldOfStudy))
                    errors[$"Educations[{i}].FieldOfStudy"] = "Field of study is required.";
                if (e.GraduationYear < 1950 || e.GraduationYear > DateTime.Now.Year + 6)
                    errors[$"Educations[{i}].GraduationYear"] = "Enter a valid graduation year.";
            }

            return errors;
        }

        private static Dictionary<string, string> ValidateSkills(SkillsViewModel vm)
        {
            var errors = new Dictionary<string, string>();
            var entries = NormalizeSkillEntries(vm);

            if (entries.Count == 0)
            {
                errors["Skills"] = "Please add at least one skill.";
                return errors;
            }

            if (entries.Count > 30)
                errors["Skills"] = "You may add up to 30 skills.";

            var duplicateSkills = entries
                .GroupBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicateSkills.Count > 0)
                errors["Skills"] = $"Duplicate skill: {string.Join(", ", duplicateSkills)}.";

            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];

                if (string.IsNullOrWhiteSpace(entry.Name))
                    errors[$"SkillEntries[{i}].Name"] = "Please select a skill.";
                else if (!Enum.TryParse<SkillType>(entry.Name, ignoreCase: true, out _))
                    errors[$"SkillEntries[{i}].Name"] = $"Invalid skill: {entry.Name}.";

                if (string.IsNullOrWhiteSpace(entry.ProficiencyLevel))
                    errors[$"SkillEntries[{i}].ProficiencyLevel"] = "Please select a proficiency level.";

                if (entry.YearsOfExperience <= 0)
                    errors[$"SkillEntries[{i}].YearsOfExperience"] = "Please enter years of experience.";
                else if (entry.YearsOfExperience < 0 || entry.YearsOfExperience > 60)
                    errors[$"SkillEntries[{i}].YearsOfExperience"] = "Enter a value between 0 and 60.";

                if (!string.IsNullOrWhiteSpace(entry.AdditionalNotes) && entry.AdditionalNotes.Length > 500)
                    errors[$"SkillEntries[{i}].AdditionalNotes"] = "Max 500 characters.";
            }

            return errors;
        }

        private static List<SkillEntryViewModel> NormalizeSkillEntries(SkillsViewModel vm)
        {
            if (vm.SkillEntries.Count > 0)
                return vm.SkillEntries
                    .Where(entry => entry != null)
                    .Select(entry =>
                    {
                        entry.Name = entry.Name?.Trim() ?? string.Empty;
                        entry.ProficiencyLevel = entry.ProficiencyLevel?.Trim();
                        entry.AdditionalNotes = entry.AdditionalNotes?.Trim();
                        return entry;
                    })
                    .ToList();

            return vm.SkillNames
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => new SkillEntryViewModel { Name = name.Trim() })
                .ToList();
        }
    }
}
