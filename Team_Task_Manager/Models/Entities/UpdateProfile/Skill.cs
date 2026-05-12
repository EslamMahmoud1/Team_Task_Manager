using System.ComponentModel.DataAnnotations.Schema;

namespace Team_Task_Manager.Models.Entities.UpdateProfile
{
    public class Skill
    {
        public int Id { get; set; }
        public SkillType Name { get; set; }

        // Navigation
        public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    }

    public class UserSkill
    {
        public int UserProfileId { get; set; }
        public int SkillId { get; set; }

        public string ProficiencyLevel { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string? AdditionalNotes { get; set; }

        // Navigation
        public UserProfile UserProfile { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }
    public enum SkillType
    {
        CSharp,
        Java,
        Python,
        JavaScript,
        SQL,
        HTML,
        CSS,
        React,
        Angular,
        Vue,
        NodeJS,
        Docker,
        Kubernetes,
        AWS,
        Azure,
        GCP,
        Git,
        CI_CD,
        AgileMethodologies
    }
}
