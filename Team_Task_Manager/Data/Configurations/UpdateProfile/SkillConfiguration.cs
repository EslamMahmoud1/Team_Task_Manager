// ================================================================
//  Data/Configurations/SkillConfiguration.cs
//  Covers both the Skill lookup table and the UserSkill join table
// ================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Team_Task_Manager.Models.Entities.UpdateProfile;

namespace YourApp.Data.Configurations
{
    // ────────────────────────────────────────────────────────────
    //  Skill  (shared lookup / tag table)
    // ────────────────────────────────────────────────────────────
    public class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            // ── Table ────────────────────────────────────────────
            builder.ToTable("Skills");

            // ── Primary Key ──────────────────────────────────────
            builder.HasKey(s => s.Id);

            // ── Properties ───────────────────────────────────────
            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(80);

            // ── Indexes ──────────────────────────────────────────
            builder.HasIndex(s => s.Name)
                   .IsUnique();  // skill names are unique tags

            // ── Relations ────────────────────────────────────────

            // One-to-many: Skill → UserSkills
            // (inverse side; primary side declared in UserSkillConfiguration)
            builder.HasMany(s => s.UserSkills)
                   .WithOne(us => us.Skill)
                   .HasForeignKey(us => us.SkillId)
                   .OnDelete(DeleteBehavior.Restrict); // keep tag rows when a user is deleted
        }
    }

    // ────────────────────────────────────────────────────────────
    //  UserSkill  (many-to-many join: UserProfile ↔ Skill)
    // ────────────────────────────────────────────────────────────
    public class UserSkillConfiguration : IEntityTypeConfiguration<UserSkill>
    {
        public void Configure(EntityTypeBuilder<UserSkill> builder)
        {
            // ── Table ────────────────────────────────────────────
            builder.ToTable("UserSkills");

            // ── Composite Primary Key ────────────────────────────
            builder.HasKey(us => new { us.UserProfileId, us.SkillId });

            // ── Properties ───────────────────────────────────────
            builder.Property(us => us.UserProfileId)
                   .IsRequired();

            builder.Property(us => us.SkillId)
                   .IsRequired();

            builder.Property(us => us.YearsOfExperience)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(us => us.AdditionalNotes)
                   .HasMaxLength(500);

            builder.HasIndex(us => us.UserProfileId);
            builder.HasIndex(us => us.SkillId);

            // ── Relations ────────────────────────────────────────

            // Many-to-one: UserSkill → UserProfile
            builder.HasOne(us => us.UserProfile)
                   .WithMany(p => p.Skills)
                   .HasForeignKey(us => us.UserProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Many-to-one: UserSkill → Skill
            builder.HasOne(us => us.Skill)
                   .WithMany(s => s.UserSkills)
                   .HasForeignKey(us => us.SkillId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
