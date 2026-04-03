using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Identity.Domain.AcessManagement.Models;

namespace Modules.Identity.Infrastructure.Database.Configurations
{
    internal sealed class IdentityTypeDefaultRoleConfiguration : IEntityTypeConfiguration<IdentityTypeDefaultRole>
    {
        public void Configure(EntityTypeBuilder<IdentityTypeDefaultRole> builder)
        {
            builder.ToTable("IdentityTypeDefaultRoles");

            builder.HasKey(x => new { x.IdentityType, x.RoleName });

            builder.Property(x => x.IdentityType)
                .HasConversion<string>()
                .HasColumnType("VARCHAR(30)")
                .IsRequired();

            builder.Property(x => x.RoleName)
                .HasColumnType("VARCHAR(50)")
                .IsRequired();

            builder
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(x => x.RoleName)
                .OnDelete(DeleteBehavior.ClientNoAction);
        }
    }
}