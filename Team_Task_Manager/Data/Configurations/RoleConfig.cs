using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Team_Task_Manager.Models.Entities.Role;

namespace Team_Task_Manager.Data.Configurations
{
    public class RoleConfig : IEntityTypeConfiguration<UserRoles>
    {
        public void Configure(EntityTypeBuilder<UserRoles> builder)
        {

            builder.HasMany(r => r.RolePermissions)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.RoleId);

            builder.HasMany(r => r.TaskUsers)
                .WithOne(u => u.UserRole)
                .HasForeignKey(u => u.UserRoleId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
