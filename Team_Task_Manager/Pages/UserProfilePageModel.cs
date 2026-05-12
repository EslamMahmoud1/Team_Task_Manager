// ================================================================
//  Pages/UserProfile.cshtml.cs  (updated)
//  Uses ViewModels + EF Core to persist all 3 sections
// ================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Team_Task_Manager.Data;
using Team_Task_Manager.Models.Entities.UpdateProfile;
using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.ViewModels.UpdateProfile;

namespace Team_Task_Manager.Pages
{
    [Authorize]  // must be logged in
    public class UserProfileModel : PageModel
    {
        private readonly TaskAppDbContext _db;
        private readonly UserManager<TaskUser> _userManager;

        public UserProfileModel(TaskAppDbContext db, UserManager<TaskUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // Bound to the form; split into sub-VMs per section
        [BindProperty] public PersonalInfoViewModel PersonalInfo { get; set; } = new();
        [BindProperty] public List<EducationViewModel> Educations { get; set; } = new();
        [BindProperty] public SkillsViewModel Skills { get; set; } = new();

        // ── GET ─────────────────────────────────────────────────
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            var profile = await _db.UserProfiles
                .Include(p => p.Educations)
                .Include(p => p.Skills).ThenInclude(s => s.Skill)
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (profile != null)
            {
                var vm = profile.ToViewModel();
                PersonalInfo = vm.PersonalInfo;
                PersonalInfo.Email = user.Email;   // always from Identity
                Educations = vm.Educations;
                Skills = vm.Skills;
            }
            else
            {
                // Pre-fill email from Identity for display
                PersonalInfo.Email = user.Email;
            }

            return Page();
        }

        // ════════════════════════════════════════════════════════
        //  AJAX Handler: Section 1 – Personal Info
        //  POST /UserProfile?handler=ValidateSection1
        // ════════════════════════════════════════════════════════
        public async Task<IActionResult> OnPostValidateSection1Async()
        {
            // Validate only PersonalInfo fields
            var errors = ValidatePersonalInfo(PersonalInfo);
            if (errors.Count > 0)
                return new JsonResult(new { success = false, errors });

            // Persist section 1
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var profile = await GetOrCreateProfile(user.Id);
            profile.ApplyFrom(PersonalInfo);
            await _db.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

        // ════════════════════════════════════════════════════════
        //  AJAX Handler: Section 2 – Education
        //  POST /UserProfile?handler=ValidateSection2
        // ════════════════════════════════════════════════════════
        public async Task<IActionResult> OnPostValidateSection2Async(
            [FromBody] List<EducationViewModel> educations)
        {
            var errors = new Dictionary<string, string>();

            if (educations == null || educations.Count == 0)
            {
                errors["general"] = "Please add at least one education entry.";
                return new JsonResult(new { success = false, errors });
            }

            for (int i = 0; i < educations.Count; i++)
            {
                var e = educations[i];
                if (string.IsNullOrWhiteSpace(e.Institution))
                    errors[$"Educations[{i}].Institution"] = "Institution name is required.";
                if (string.IsNullOrWhiteSpace(e.Degree))
                    errors[$"Educations[{i}].Degree"] = "Degree is required.";
                if (string.IsNullOrWhiteSpace(e.FieldOfStudy))
                    errors[$"Educations[{i}].FieldOfStudy"] = "Field of study is required.";
                if (e.GraduationYear < 1950 || e.GraduationYear > DateTime.Now.Year + 6)
                    errors[$"Educations[{i}].GraduationYear"] = "Enter a valid graduation year.";
            }

            if (errors.Count > 0)
                return new JsonResult(new { success = false, errors });

            // Persist education entries (upsert)
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var profile = await GetOrCreateProfile(user.Id);
            await UpsertEducationsAsync(profile.Id, educations);

            return new JsonResult(new { success = true });
        }

        // ════════════════════════════════════════════════════════
        //  AJAX Handler: Section 3 – Skills
        //  POST /UserProfile?handler=ValidateSection3
        // ════════════════════════════════════════════════════════
        public async Task<IActionResult> OnPostValidateSection3Async()
        {
            var errors = new Dictionary<string, string>();

            // Parse skill names from JSON string sent by JS
            var skillNames = ParseSkillNames(Request.Form["SkillsJson"]);

            if (skillNames.Count == 0)
                errors["Skills"] = "Please add at least one skill.";
            else if (skillNames.Count > 30)
                errors["Skills"] = "You may add up to 30 skills.";

            if (string.IsNullOrWhiteSpace(Skills.ProficiencyLevel))
                errors["ProficiencyLevel"] = "Please select a proficiency level.";

            if (Skills.YearsOfExperience < 0 || Skills.YearsOfExperience > 60)
                errors["YearsOfExperience"] = "Enter a valid number of years (0–60).";

            if (errors.Count > 0)
                return new JsonResult(new { success = false, errors });

            // Persist skills
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var profile = await GetOrCreateProfile(user.Id);
            await UpsertSkillsAsync(profile.Id, skillNames, Skills);

            return new JsonResult(new { success = true });
        }

        // ════════════════════════════════════════════════════════
        //  Full Submit
        //  POST /UserProfile?handler=Submit
        // ════════════════════════════════════════════════════════
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var profile = await _db.UserProfiles
                .Include(p => p.Educations)
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (profile == null)
                return new JsonResult(new { success = false, message = "Profile not found. Please complete all sections." });

            profile.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

        // ════════════════════════════════════════════════════════
        //  Save Draft (no validation)
        //  POST /UserProfile?handler=SaveDraft
        // ════════════════════════════════════════════════════════
        public async Task<IActionResult> OnPostSaveDraftAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var profile = await GetOrCreateProfile(user.Id);
            profile.ApplyFrom(PersonalInfo);
            await _db.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Draft saved." });
        }

        // ════════════════════════════════════════════════════════
        //  Private helpers
        // ════════════════════════════════════════════════════════

        /// <summary>Gets the user's profile, or creates + tracks a new one.</summary>
        private async Task<UserProfile> GetOrCreateProfile(long userId)
        {
            var profile = await _db.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                profile = new UserProfile { UserId = userId };
                _db.UserProfiles.Add(profile);
                await _db.SaveChangesAsync(); // get the Id before child inserts
            }

            return profile;
        }

        /// <summary>Upserts education rows: update existing, insert new, delete removed.</summary>
        private async Task UpsertEducationsAsync(int profileId, List<EducationViewModel> vms)
        {
            var existing = await _db.Educations
                .Where(e => e.UserProfileId == profileId)
                .ToListAsync();

            var submittedIds = vms.Where(v => v.Id.HasValue).Select(v => v.Id!.Value).ToHashSet();

            // Delete removed entries
            var toDelete = existing.Where(e => !submittedIds.Contains(e.Id)).ToList();
            _db.Educations.RemoveRange(toDelete);

            foreach (var vm in vms)
            {
                if (vm.Id.HasValue)
                {
                    // Update
                    var row = existing.FirstOrDefault(e => e.Id == vm.Id.Value);
                    if (row != null)
                    {
                        row.Institution = vm.Institution;
                        row.Degree = vm.Degree;
                        row.FieldOfStudy = vm.FieldOfStudy;
                        row.GraduationYear = vm.GraduationYear;
                        row.Description = vm.Description;
                    }
                }
                else
                {
                    // Insert
                    _db.Educations.Add(vm.ToEntity(profileId));
                }
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>Syncs skill tags: resolves or creates Skill rows, then upserts UserSkill join rows.</summary>
        private async Task UpsertSkillsAsync(int profileId, List<string> skillNames, SkillsViewModel vm)
        {
            // Remove all existing UserSkill rows for this profile
            var existing = await _db.UserSkills
                .Where(us => us.UserProfileId == profileId)
                .ToListAsync();
            _db.UserSkills.RemoveRange(existing);

            foreach (var name in skillNames.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                // Find or create the Skill tag
                var skill = await _db.Skills.FirstOrDefaultAsync(s => s.Name.ToString() == name)
                            ?? new Skill { Name = Enum.Parse<SkillType>(name) };

                if (skill.Id == 0) _db.Skills.Add(skill);
                await _db.SaveChangesAsync(); // ensure skill.Id is set

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

        private static Dictionary<string, string> ValidatePersonalInfo(PersonalInfoViewModel vm)
        {
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(vm.FirstName)) errors["FirstName"] = "First name is required.";
            else if (vm.FirstName.Length > 50) errors["FirstName"] = "Max 50 characters.";
            if (string.IsNullOrWhiteSpace(vm.LastName)) errors["LastName"] = "Last name is required.";
            else if (vm.LastName.Length > 50) errors["LastName"] = "Max 50 characters.";
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

        private static List<string> ParseSkillNames(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new();
            try { return System.Text.Json.JsonSerializer.Deserialize<List<string>>(json) ?? new(); }
            catch { return new(); }
        }
    }
}
