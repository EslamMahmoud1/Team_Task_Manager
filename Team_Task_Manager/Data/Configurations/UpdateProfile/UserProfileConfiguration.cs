// ================================================================
//  Data/Configurations/UserProfileConfiguration.cs
// ================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Team_Task_Manager.Models.Entities.UpdateProfile;

namespace YourApp.Data.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            // ── Table ────────────────────────────────────────────
            builder.ToTable("UserProfiles");

            // ── Primary Key ──────────────────────────────────────
            builder.HasKey(p => p.Id);

            // ── Properties ───────────────────────────────────────
            builder.Property(p => p.UserId)
                   .IsRequired();

            builder.Property(p => p.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(p => p.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(p => p.Phone)
                   .HasMaxLength(20);

            builder.Property(p => p.Location)
                   .HasMaxLength(100);

            builder.Property(p => p.Headline)
                   .HasMaxLength(120);

            builder.Property(p => p.Bio)
                   .HasMaxLength(1000);

            builder.Property(p => p.ProfilePictureUrl)
                   .HasMaxLength(500);

            builder.Property(p => p.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.UpdatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            // ── Indexes ──────────────────────────────────────────
            builder.HasIndex(p => p.UserId)
                   .IsUnique();  // enforce one profile per user

            // ── Relations ────────────────────────────────────────

            // One-to-one: ApplicationUser → UserProfile
            builder.HasOne(p => p.User)
                   .WithOne(u => u.Profile)
                   .HasForeignKey<UserProfile>(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // One-to-many: UserProfile → Educations
            builder.HasMany(p => p.Educations)
                   .WithOne(e => e.UserProfile)
                   .HasForeignKey(e => e.UserProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

            // One-to-many: UserProfile → UserSkills (join)
            builder.HasMany(p => p.Skills)
                   .WithOne(us => us.UserProfile)
                   .HasForeignKey(us => us.UserProfileId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
