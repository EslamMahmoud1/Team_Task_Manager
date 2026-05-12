using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Team_Task_Manager.Views.UpdateProfile
{
    public class UserProfileModel : PageModel
    {
        [BindProperty]
        public ProfileInputModel Input { get; set; } = new();

        public void OnGet()
        {
            // Pre-populate with existing user data if available
            // Input = await _userService.GetCurrentUserProfileAsync(User);
            Input.Email = "jane.doe@email.com"; // Example: email locked
        }

        // ─────────────────────────────────────────────
        // AJAX Handler: Validate Section 1 – Personal Info
        // POST /UserProfile?handler=ValidateSection1
        // ─────────────────────────────────────────────
        public IActionResult OnPostValidateSection1()
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(Input.FirstName))
                errors["FirstName"] = "First name is required.";
            else if (Input.FirstName.Length > 50)
                errors["FirstName"] = "First name must be 50 characters or fewer.";

            if (string.IsNullOrWhiteSpace(Input.LastName))
                errors["LastName"] = "Last name is required.";
            else if (Input.LastName.Length > 50)
                errors["LastName"] = "Last name must be 50 characters or fewer.";

            if (string.IsNullOrWhiteSpace(Input.Phone))
                errors["Phone"] = "Phone number is required.";
            else if (!System.Text.RegularExpressions.Regex.IsMatch(Input.Phone, @"^\+?[\d\s\-().]{7,20}$"))
                errors["Phone"] = "Enter a valid phone number.";

            if (string.IsNullOrWhiteSpace(Input.Location))
                errors["Location"] = "Location is required.";

            if (Input.DateOfBirth == default || Input.DateOfBirth > DateOnly.FromDateTime(DateTime.Today.AddYears(-13)))
                errors["DateOfBirth"] = "Please enter a valid date of birth (must be 13+).";

            if (string.IsNullOrWhiteSpace(Input.Headline))
                errors["Headline"] = "Headline is required.";
            else if (Input.Headline.Length > 120)
                errors["Headline"] = "Headline must be 120 characters or fewer.";

            if (!string.IsNullOrWhiteSpace(Input.Bio) && Input.Bio.Length > 1000)
                errors["Bio"] = "Bio must be 1000 characters or fewer.";

            if (errors.Count > 0)
                return new JsonResult(new { success = false, errors });

            // Optionally save draft to DB here
            // await _userService.SavePersonalInfoAsync(Input);

            return new JsonResult(new { success = true });
        }

        // ─────────────────────────────────────────────
        // AJAX Handler: Validate Section 2 – Education
        // POST /UserProfile?handler=ValidateSection2
        // ─────────────────────────────────────────────
        public IActionResult OnPostValidateSection2([FromBody] List<EducationEntry> educations)
        {
            var errors = new Dictionary<string, string>();

            if (educations == null || educations.Count == 0)
            {
                errors["general"] = "Please add at least one education entry.";
                return new JsonResult(new { success = false, errors });
            }

            for (int i = 0; i < educations.Count; i++)
            {
                var entry = educations[i];

                if (string.IsNullOrWhiteSpace(entry.Institution))
                    errors[$"Educations[{i}].Institution"] = "Institution name is required.";

                if (string.IsNullOrWhiteSpace(entry.Degree))
                    errors[$"Educations[{i}].Degree"] = "Degree is required.";

                if (string.IsNullOrWhiteSpace(entry.FieldOfStudy))
                    errors[$"Educations[{i}].FieldOfStudy"] = "Field of study is required.";

                if (entry.GraduationYear < 1950 || entry.GraduationYear > DateTime.Now.Year + 6)
                    errors[$"Educations[{i}].GraduationYear"] = "Enter a valid graduation year.";
            }

            if (errors.Count > 0)
                return new JsonResult(new { success = false, errors });

            return new JsonResult(new { success = true });
        }

        // ─────────────────────────────────────────────
        // AJAX Handler: Validate Section 3 – Skills
        // POST /UserProfile?handler=ValidateSection3
        // ─────────────────────────────────────────────
        public IActionResult OnPostValidateSection3()
        {
            var errors = new Dictionary<string, string>();

            // Skills come as JSON string in SkillsJson
            List<string>? skills = null;
            if (!string.IsNullOrWhiteSpace(Input.SkillsJson))
            {
                try { skills = JsonSerializer.Deserialize<List<string>>(Input.SkillsJson); }
                catch { errors["Skills"] = "Invalid skills data."; }
            }

            if (skills == null || skills.Count == 0)
                errors["Skills"] = "Please add at least one skill.";
            else if (skills.Count > 30)
                errors["Skills"] = "You may add up to 30 skills.";

            if (string.IsNullOrWhiteSpace(Input.ProficiencyLevel))
                errors["ProficiencyLevel"] = "Please select a proficiency level.";

            if (Input.YearsOfExperience < 0 || Input.YearsOfExperience > 60)
                errors["YearsOfExperience"] = "Enter a valid number of years (0–60).";

            if (errors.Count > 0)
                return new JsonResult(new { success = false, errors });

            return new JsonResult(new { success = true });
        }

        // ─────────────────────────────────────────────
        // Full Submit: POST /UserProfile?handler=Submit
        // ─────────────────────────────────────────────
        public IActionResult OnPostSubmit()
        {
            if (!ModelState.IsValid)
                return new JsonResult(new { success = false, message = "Validation failed." });

            // TODO: Persist full profile to database
            // await _userService.SaveFullProfileAsync(Input);

            return new JsonResult(new { success = true });
        }

        // ─────────────────────────────────────────────
        // Save Draft: POST /UserProfile?handler=SaveDraft
        // ─────────────────────────────────────────────
        public IActionResult OnPostSaveDraft()
        {
            // Save partial data without full validation
            // await _userService.SaveDraftAsync(Input);
            return new JsonResult(new { success = true, message = "Draft saved." });
        }
    }

    // ─────────────────────────────────────────────
    // Input Models
    // ─────────────────────────────────────────────
    public class ProfileInputModel
    {
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Display(Name = "Date of Birth")]
        public DateOnly DateOfBirth { get; set; }

        [Display(Name = "Headline")]
        [MaxLength(120)]
        public string? Headline { get; set; }

        [Display(Name = "Bio")]
        [MaxLength(1000)]
        public string? Bio { get; set; }

        // Education is handled via JSON body in Section 2 handler
        public List<EducationEntry>? Educations { get; set; }

        // Skills stored as JSON string
        public string? SkillsJson { get; set; }

        [Display(Name = "Proficiency Level")]
        public string? ProficiencyLevel { get; set; }

        [Display(Name = "Years of Experience")]
        [Range(0, 60)]
        public int YearsOfExperience { get; set; }

        [Display(Name = "Additional Notes")]
        public string? AdditionalNotes { get; set; }
    }

    public class EducationEntry
    {
        public string? Institution { get; set; }
        public string? Degree { get; set; }
        public string? FieldOfStudy { get; set; }
        public int GraduationYear { get; set; }
        public string? Description { get; set; }
    }
}
