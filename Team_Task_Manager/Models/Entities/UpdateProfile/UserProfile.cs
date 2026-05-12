using Team_Task_Manager.Models.Entities.User;

namespace Team_Task_Manager.Models.Entities.UpdateProfile
{
    public class UserProfile
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Headline { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public TaskUser User { get; set; } = null!;
        public ICollection<Education> Educations { get; set; } = new List<Education>();
        public ICollection<UserSkill> Skills { get; set; } = new List<UserSkill>();
    }
}
