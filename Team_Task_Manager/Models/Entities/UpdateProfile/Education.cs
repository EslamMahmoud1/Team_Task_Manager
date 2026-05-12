namespace Team_Task_Manager.Models.Entities.UpdateProfile
{
    public class Education
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public string Institution { get; set; } = null!;
        public string Degree { get; set; } = null!;
        public string FieldOfStudy { get; set; } = null!;
        public int GraduationYear { get; set; }
        public string? Description { get; set; }

        // Navigation
        public UserProfile UserProfile { get; set; } = null!;
    }
}
