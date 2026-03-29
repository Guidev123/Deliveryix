using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Identity.Domain.Identities.Models;

namespace Modules.Identity.Infrastructure.Database.Configurations
{
    internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.Name);

            builder.Property(r => r.Name)
                .HasColumnType("VARCHAR(50)")
                .IsRequired();

            builder
                .HasMany<Domain.Identities.Entities.Identity>()
                .WithMany(u => u.Roles)
                .UsingEntity(joinBuilder =>
                {
                    joinBuilder.ToTable("IdentityRoles");

                    joinBuilder.Property("RolesName").HasColumnName("RoleName");
                });
        }
    }
}