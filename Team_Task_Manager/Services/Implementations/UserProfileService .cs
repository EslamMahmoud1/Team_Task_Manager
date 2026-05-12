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

        // ── Section 1: Personal Info ─────────────────────────────
        public async Task<Result<string>> SavePersonalInfoAsync(long userId, PersonalInfoViewModel vm)
        {
            var errors = ValidatePersonalInfo(vm);
            if (errors.Count > 0)
                return Result<string>.Failure(errors.Values.ToList());

            var profile = await GetOrCreateProfileAsync(userId, vm);
            profile.ApplyFrom(vm);

            await _db.SaveChangesAsync();
            return Result<string>.Success("Personal information saved successfully.");
        }

        // ── Section 2: Education ─────────────────────────────────
        public async Task<Result<string>> SaveEducationsAsync(long userId, List<EducationViewModel> vms)
        {
            var errors = ValidateEducations(vms);
            if (errors.Count > 0)
                return Result<string>.Failure(errors.Values.ToList());
            var profile = await GetOrCreateProfileAsync(userId);
            await EnsureProfileIsSavedAsync(profile);

            await UpsertEducationsAsync(profile.Id, vms);
            return Result<string>.Success("Educations saved successfully.");
        }

        // ── Section 3: Skills ────────────────────────────────────
        public async Task<Result<string>> SaveSkillsAsync(long userId, SkillsViewModel vm)
        {
            var errors = ValidateSkills(vm);
            if (errors.Count > 0)
                return Result<string>.Failure(errors.Values.ToList());

            var profile = await GetOrCreateProfileAsync(userId);
            await EnsureProfileIsSavedAsync(profile);

            await UpsertSkillsAsync(profile.Id, vm);
            return Result<string>.Success("Skills saved successfully.");
        }

        // ── Save Draft (no validation) ───────────────────────────
        public async Task<Result<string>> SaveDraftAsync(long userId, PersonalInfoViewModel vm)
        {
            var profile = await GetOrCreateProfileAsync(userId, vm);
            profile.ApplyFrom(vm);

            await _db.SaveChangesAsync();
            return Result<string>.Success("Draft saved.");
        }

        // ════════════════════════════════════════════════════════
        //  Private: DB helpers
        // ════════════════════════════════════════════════════════

        private async Task<UserProfile> GetOrCreateProfileAsync(
            long userId,
            PersonalInfoViewModel? personalInfo = null)
        {
            var profile = await _db.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                var now = DateTime.UtcNow;

                profile = new UserProfile
                {
                    UserId = userId,
                    FirstName = personalInfo?.FirstName?.Trim() ?? string.Empty,
                    LastName = personalInfo?.LastName?.Trim() ?? string.Empty,
                    Phone = personalInfo?.Phone,
                    Location = personalInfo?.Location,
                    DateOfBirth = personalInfo?.DateOfBirth,
                    Headline = personalInfo?.Headline,
                    Bio = personalInfo?.Bio,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                _db.UserProfiles.Add(profile);
            }

            return profile;
        }

        private async Task EnsureProfileIsSavedAsync(UserProfile profile)
        {
            if (profile.Id == 0)
                await _db.SaveChangesAsync();
        }

        private async Task UpsertEducationsAsync(int profileId, List<EducationViewModel> vms)
        {
            var existing = await _db.Educations
                .Where(e => e.UserProfileId == profileId)
                .ToListAsync();

            var submittedIds = vms
                .Where(v => v.Id.HasValue)
                .Select(v => v.Id!.Value)
                .ToHashSet();

            // Delete removed entries
            _db.Educations.RemoveRange(
                existing.Where(e => !submittedIds.Contains(e.Id))
            );

            foreach (var vm in vms)
            {
                if (vm.Id.HasValue)
                {
                    var row = existing.FirstOrDefault(e => e.Id == vm.Id.Value);
                    if (row != null) row.ApplyFrom(vm);
                }
                else
                {
                    _db.Educations.Add(vm.ToEntity(profileId));
                }
            }

            await _db.SaveChangesAsync();
        }

        private async Task UpsertSkillsAsync(int profileId, SkillsViewModel vm)
        {
            // Remove all existing skill rows for this profile and re-sync
            var existing = await _db.UserSkills
                .Where(us => us.UserProfileId == profileId)
                .ToListAsync();

            _db.UserSkills.RemoveRange(existing);

            foreach (var name in vm.SkillNames.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var skillType = Enum.Parse<SkillType>(name, ignoreCase: true);
                var skill = await _db.Skills.FirstOrDefaultAsync(s => s.Name == skillType);
                if (skill == null)
                {
                    skill = new Skill { Name = skillType };
                    _db.Skills.Add(skill);
                    await _db.SaveChangesAsync(); // get skill.Id before using it
                }

                _db.UserSkills.Add(new UserSkill
                {
                    UserProfileId = profileId,
                    SkillId = skill.Id,
                    YearsOfExperience = vm.YearsOfExperience,
                    AdditionalNotes = vm.AdditionalNotes,
                });
            }

            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════
        //  Private: Validation
        // ════════════════════════════════════════════════════════

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

            if (!string.IsNullOrWhiteSpace(vm.Phone) &&
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

            if (vm.SkillNames == null || vm.SkillNames.Count == 0)
                errors["Skills"] = "Please add at least one skill.";
            else if (vm.SkillNames.Count > 30)
                errors["Skills"] = "You may add up to 30 skills.";
            else
            {
                var invalidSkills = vm.SkillNames
                    .Where(name => !Enum.TryParse<SkillType>(name, ignoreCase: true, out _))
                    .ToList();

                if (invalidSkills.Count > 0)
                    errors["Skills"] = $"Invalid skill: {string.Join(", ", invalidSkills)}.";
            }

            if (string.IsNullOrWhiteSpace(vm.ProficiencyLevel))
                errors["ProficiencyLevel"] = "Please select a proficiency level.";

            if (vm.YearsOfExperience < 0 || vm.YearsOfExperience > 60)
                errors["YearsOfExperience"] = "Enter a value between 0 and 60.";

            return errors;
        }
    }
}
