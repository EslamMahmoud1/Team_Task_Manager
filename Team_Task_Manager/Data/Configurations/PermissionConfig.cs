using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Team_Task_Manager.Models.Entities.Permissions;

namespace Team_Task_Manager.Data.Configurations
{
    public class PermissionConfig : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasMany(p => p.RolePermissions)
                   .WithOne(rp => rp.Permission)
                   .HasForeignKey(rp => rp.PermissionId);

            builder.Property(p => p.Name)
            .HasConversion<string>();
        }
    }
}
