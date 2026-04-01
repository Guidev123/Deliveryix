using Deliveryix.Commons.Application.Outbox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deliveryix.Commons.Infrastructure.Outbox.Configurations
{
    public sealed class OutboxMessageConsumerConfiguration : IEntityTypeConfiguration<OutboxMessageConsumer>
    {
        public void Configure(EntityTypeBuilder<OutboxMessageConsumer> builder)
        {
            builder.ToTable("OutboxMessageConsumers");

            builder.HasKey(c => new { c.Id, c.Name });

            builder.Property(c => c.Name)
                .HasColumnType("VARCHAR(256)")
                .IsRequired();
        }
    }
}