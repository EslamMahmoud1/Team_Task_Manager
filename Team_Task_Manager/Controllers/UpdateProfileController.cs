using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.Shared;
using Team_Task_Manager.ViewModels.UpdateProfile;

namespace Team_Task_Manager.Controllers
{
    public class UpdateProfileController : Controller
    {
        private readonly IUserProfileService _profileService;
        private readonly UserManager<TaskUser> _userManager;

        public UpdateProfileController(
            IUserProfileService profileService,
            UserManager<TaskUser> userManager)
        {
            _profileService = profileService;
            _userManager = userManager;
        }

        // ── Helpers ─────────────────────────────────────────────
        private long UserId => long.Parse(_userManager.GetUserId(User)!);

        // ════════════════════════════════════════════════════════
        //  GET /profile
        //  Load profile page with all 3 sections pre-populated
        // ════════════════════════════════════════════════════════
        
        public async Task<IActionResult> Index()
        {
            var vm = await _profileService.GetProfileAsync(UserId)
                     ?? new UserProfileViewModel();

            // Email is always read from Identity, never from profile table
            vm.PersonalInfo.Email = _userManager.GetUserName(User);

            return View(vm);
        }

        // ════════════════════════════════════════════════════════
        //  POST /profile/personal-info
        //  AJAX — validate & save Section 1
        // ════════════════════════════════════════════════════════
        [HttpPost]
        public async Task<IActionResult> SavePersonalInfo([FromForm] PersonalInfoViewModel vm)
        {
            var result = await _profileService.SavePersonalInfoAsync(UserId, vm);
            return Json(result.ToApiResponse());
        }

        // ════════════════════════════════════════════════════════
        //  POST /profile/education
        //  AJAX — validate & save Section 2
        //  Receives a JSON array of education entries from JS
        // ════════════════════════════════════════════════════════
        [HttpPost]
        public async Task<IActionResult> SaveEducation([FromBody] List<EducationViewModel> educations)
        {
            if (educations == null || educations.Count == 0)
                return Json(new { success = false, errors = new { general = "Please add at least one education entry." } });

            var result = await _profileService.SaveEducationsAsync(UserId, educations);
            return Json(result.ToApiResponse());
        }

        // ════════════════════════════════════════════════════════
        //  POST /profile/skills
        //  AJAX — validate & save Section 3
        // ════════════════════════════════════════════════════════
        [HttpPost]
        public async Task<IActionResult> SaveSkills([FromForm] SkillsViewModel vm)
        {
            // Parse skill names from the JSON string sent by JS
            vm.SkillNames = ParseSkillNames(Request.Form["SkillsJson"]);

            var result = await _profileService.SaveSkillsAsync(UserId, vm);
            return Json(result.ToApiResponse());
        }

        // ════════════════════════════════════════════════════════
        //  POST /profile/draft
        //  AJAX — save partial data without full validation
        // ════════════════════════════════════════════════════════
        [HttpPost]
        public async Task<IActionResult> SaveDraft([FromForm] PersonalInfoViewModel vm)
        {
            var result = await _profileService.SaveDraftAsync(UserId, vm);
            return Json(result.ToApiResponse());
        }

        // ════════════════════════════════════════════════════════
        //  POST /profile/photo
        //  Upload profile picture — handled separately from form
        // ════════════════════════════════════════════════════════
        [HttpPost]
        public async Task<IActionResult> UploadPhoto(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
                return Json(new { success = false, errors = new { Photo = "Please select a photo." } });

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(photo.ContentType.ToLower()))
                return Json(new { success = false, errors = new { Photo = "Only JPG, PNG, or WEBP images are allowed." } });

            // Validate file size (2MB max)
            if (photo.Length > 2 * 1024 * 1024)
                return Json(new { success = false, errors = new { Photo = "Photo must be under 2MB." } });

            // TODO: Save to blob storage (Azure / S3) and get back the URL
            // var url = await _storageService.UploadAsync(photo);
            var url = $"/uploads/profiles/{UserId}{Path.GetExtension(photo.FileName)}";

            // Save URL to profile
            var profile = await _profileService.GetProfileAsync(UserId);
            if (profile != null)
            {
                // Update photo URL via service (add method if needed)
            }

            return Json(new { success = true, url });
        }

        // ════════════════════════════════════════════════════════
        //  Helpers
        // ════════════════════════════════════════════════════════

        private static List<string> ParseSkillNames(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new();
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<string>>(json) ?? new();
            }
            catch { return new(); }
        }
    }

    // ════════════════════════════════════════════════════════════
    //  Extension: map ServiceResult → JSON-friendly shape
    // ════════════════════════════════════════════════════════════
    public static class ServiceResultExtensions
    {
        public static object ToApiResponse(this Result<string> result) => new
        {
            success = result.IsSuccess,
            message = result.Value,
            errors = result.Errors,
        };
    }
}

