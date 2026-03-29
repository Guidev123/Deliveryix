using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Identity.Domain.Identities.ValueObjects;

namespace Modules.Identity.Infrastructure.Database.Configurations
{
    internal sealed class IdentityConfiguration : IEntityTypeConfiguration<Domain.Identities.Entities.Identity>
    {
        public void Configure(EntityTypeBuilder<Domain.Identities.Entities.Identity> builder)
        {
            builder.ToTable("Identities");

            builder.HasKey(c => c.Id);

            builder.OwnsOne(c => c.Email, email =>
            {
                email.Property(c => c.Address)
                    .HasColumnType("VARCHAR(160)")
                    .HasColumnName(nameof(Email))
                    .IsRequired();

                email.HasIndex(e => e.Address)
                    .IsUnique()
                    .HasDatabaseName("IX_Identities_Email");
            });

            builder.OwnsOne(c => c.Phone, phone =>
            {
                phone.Property(c => c.Number)
                    .HasColumnType("VARCHAR(40)")
                    .HasColumnName(nameof(Phone))
                    .IsRequired();
            });

            builder.OwnsOne(c => c.State, identityState =>
            {
                identityState.Property(c => c.Status)
                    .HasConversion<string>()
                    .HasColumnType("VARCHAR(80)")
                    .HasColumnName("Status")
                    .IsRequired();

                identityState.Property(c => c.DeletedOn)
                    .HasColumnName("DeletedOn")
                    .IsRequired(false);
            });

            builder.OwnsOne(c => c.Document, document =>
            {
                document.Property(c => c.Type)
                    .HasConversion<string>()
                    .HasColumnType("VARCHAR(10)")
                    .HasColumnName("DocumentType")
                    .IsRequired();

                document.Property(c => c.Number)
                    .HasColumnName("Document")
                    .HasColumnType("VARCHAR(20)")
                    .IsRequired();

                document.HasIndex(c => new { c.Type, c.Number })
                    .IsUnique();
            });

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.Property(c => c.Type)
                .HasConversion<string>()
                .HasColumnType("VARCHAR(30)")
                .IsRequired();

            builder.Property(c => c.IdentityProviderId)
                .HasColumnType("VARCHAR(60)")
                .IsRequired();

            builder.HasIndex(c => new { c.CreatedOn, c.Id });
        }
    }
}