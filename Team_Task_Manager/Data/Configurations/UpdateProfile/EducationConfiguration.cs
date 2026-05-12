// ================================================================
//  Data/Configurations/EducationConfiguration.cs
// ================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Team_Task_Manager.Models.Entities.UpdateProfile;

namespace YourApp.Data.Configurations
{
    public class EducationConfiguration : IEntityTypeConfiguration<Education>
    {
        public void Configure(EntityTypeBuilder<Education> builder)
        {
            // ── Table ────────────────────────────────────────────
            builder.ToTable("Educations");

            // ── Primary Key ──────────────────────────────────────
            builder.HasKey(e => e.Id);

            // ── Properties ───────────────────────────────────────
            builder.Property(e => e.UserProfileId)
                   .IsRequired();

            builder.Property(e => e.Institution)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(e => e.Degree)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.FieldOfStudy)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.GraduationYear)
                   .IsRequired()
                   .HasColumnType("int");

            builder.Property(e => e.Description)
                   .HasMaxLength(500);

            // ── Relations ────────────────────────────────────────

            // Many-to-one: Education → UserProfile
            // (inverse side; already declared in UserProfileConfiguration,
            //  repeated here for clarity & self-containment)
            builder.HasOne(e => e.UserProfile)
                   .WithMany(p => p.Educations)
                   .HasForeignKey(e => e.UserProfileId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
