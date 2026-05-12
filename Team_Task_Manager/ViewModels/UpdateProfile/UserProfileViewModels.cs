// ================================================================
//  Models/ViewModels/UserProfileViewModels.cs
//  ViewModels for the 3-step profile form + mapping helpers
// ================================================================

using System.ComponentModel.DataAnnotations;
using Team_Task_Manager.Models.Entities.UpdateProfile;

namespace Team_Task_Manager.ViewModels.UpdateProfile
{
    // ────────────────────────────────────────────────────────────
    // Root ViewModel (bound to the Razor Page via [BindProperty])
    // ────────────────────────────────────────────────────────────
    public class UserProfileViewModel
    {
        public PersonalInfoViewModel PersonalInfo { get; set; } = new();
        public List<EducationViewModel> Educations { get; set; } = new();
        public SkillsViewModel Skills { get; set; } = new();
    }

    // ────────────────────────────────────────────────────────────
    // Section 1 – Personal Info
    // ────────────────────────────────────────────────────────────
    public class PersonalInfoViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "Max 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Max 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>Read-only; sourced from Identity – never updated here.</summary>
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [MaxLength(20)]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [MaxLength(100)]
        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Display(Name = "Date of Birth")]
        public DateOnly? DateOfBirth { get; set; }

        [MaxLength(120, ErrorMessage = "Max 120 characters.")]
        [Display(Name = "Headline")]
        public string? Headline { get; set; }

        [MaxLength(1000, ErrorMessage = "Max 1000 characters.")]
        [Display(Name = "Bio")]
        public string? Bio { get; set; }

        /// <summary>Holds the current photo URL for display; upload handled separately.</summary>
        public string? ProfilePictureUrl { get; set; }
    }

    // ────────────────────────────────────────────────────────────
    // Section 2 – Education entry
    // ────────────────────────────────────────────────────────────
    public class EducationViewModel
    {
        public int? Id { get; set; }  // null = new entry, set = existing DB row

        [Required(ErrorMessage = "Institution name is required.")]
        [MaxLength(150)]
        [Display(Name = "Institution Name")]
        public string Institution { get; set; } = string.Empty;

        [Required(ErrorMessage = "Degree is required.")]
        [MaxLength(100)]
        [Display(Name = "Degree")]
        public string Degree { get; set; } = string.Empty;

        [Required(ErrorMessage = "Field of study is required.")]
        [MaxLength(100)]
        [Display(Name = "Field of Study")]
        public string FieldOfStudy { get; set; } = string.Empty;

        [Range(1950, 2035, ErrorMessage = "Enter a valid graduation year.")]
        [Display(Name = "Graduation Year")]
        public int GraduationYear { get; set; }

        [MaxLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }

    // ────────────────────────────────────────────────────────────
    // Section 3 – Skills
    // ────────────────────────────────────────────────────────────
    public class SkillsViewModel
    {
        /// <summary>Tag names entered by the user (sent as JSON array from JS).</summary>
        [MinLength(1, ErrorMessage = "Add at least one skill.")]
        public List<string> SkillNames { get; set; } = new();

        [Required(ErrorMessage = "Please select a proficiency level.")]
        [Display(Name = "Proficiency Level")]
        public string? ProficiencyLevel { get; set; }

        [Range(0, 60, ErrorMessage = "Enter a value between 0 and 60.")]
        [Display(Name = "Years of Experience")]
        public int YearsOfExperience { get; set; }

        [MaxLength(500)]
        [Display(Name = "Additional Notes")]
        public string? AdditionalNotes { get; set; }
    }

    // ════════════════════════════════════════════════════════════
    //  Mapping Extensions  (ViewModel  ↔  Domain entity)
    //  No AutoMapper dependency – explicit and refactor-safe.
    // ════════════════════════════════════════════════════════════
    public static class UserProfileMappings
    {
        // ── Domain → ViewModel ──────────────────────────────────

        public static UserProfileViewModel ToViewModel(this UserProfile profile)
        {
            return new UserProfileViewModel
            {
                PersonalInfo = profile.ToPersonalInfoViewModel(),
                Educations = profile.Educations.Select(e => e.ToViewModel()).ToList(),
                Skills = profile.ToSkillsViewModel(),
            };
        }

        public static PersonalInfoViewModel ToPersonalInfoViewModel(this UserProfile profile)
        {
            return new PersonalInfoViewModel
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Email = profile.User?.Email,      // from Identity
                Phone = profile.Phone,
                Location = profile.Location,
                DateOfBirth = profile.DateOfBirth,
                Headline = profile.Headline,
                Bio = profile.Bio,
                ProfilePictureUrl = profile.ProfilePictureUrl,
            };
        }

        public static EducationViewModel ToViewModel(this Education edu)
        {
            return new EducationViewModel
            {
                Id = edu.Id,
                Institution = edu.Institution,
                Degree = edu.Degree,
                FieldOfStudy = edu.FieldOfStudy,
                GraduationYear = edu.GraduationYear,
                Description = edu.Description,
            };
        }

        public static SkillsViewModel ToSkillsViewModel(this UserProfile profile)
        {
            // All skills on the profile share the same ProficiencyLevel / YearsOfExperience
            // (stored per-row; we read from the first entry for the form defaults)
            var first = profile.Skills.FirstOrDefault();
            return new SkillsViewModel
            {
                SkillNames = profile.Skills.Select(s => s.Skill.Name.ToString()).ToList(),
                YearsOfExperience = first?.YearsOfExperience ?? 0,
                AdditionalNotes = first?.AdditionalNotes,
            };
        }

        // ── ViewModel → Domain ──────────────────────────────────

        /// <summary>
        /// Apply PersonalInfo VM onto an existing (or new) UserProfile entity.
        /// </summary>
        public static void ApplyFrom(this UserProfile profile, PersonalInfoViewModel vm)
        {
            profile.FirstName = vm.FirstName;
            profile.LastName = vm.LastName;
            profile.Phone = vm.Phone;
            profile.Location = vm.Location;
            profile.DateOfBirth = vm.DateOfBirth;
            profile.Headline = vm.Headline;
            profile.Bio = vm.Bio;
            profile.UpdatedAt = DateTime.UtcNow;
            // ProfilePictureUrl is set separately after file upload
        }

        /// <summary>
        /// Convert an EducationViewModel to a new Education DB entity.
        /// </summary>
        public static Education ToEntity(this EducationViewModel vm, int userProfileId)
        {
            return new Education
            {
                Id = vm.Id ?? 0,
                UserProfileId = userProfileId,
                Institution = vm.Institution,
                Degree = vm.Degree,
                FieldOfStudy = vm.FieldOfStudy,
                GraduationYear = vm.GraduationYear,
                Description = vm.Description,
            };
        }
    }
}
