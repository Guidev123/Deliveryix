using Deliveryix.Commons.Domain.DomainObjects;
using Deliveryix.Commons.Infrastructure.Inbox.Configurations;
using Deliveryix.Commons.Infrastructure.Outbox.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Modules.Identity.Infrastructure.Database
{
    internal sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> context) : DbContext(context)
    {
        public DbSet<Domain.Identities.Entities.Identity> Identities { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schemas.DefaultSchemaName);

            modelBuilder.Ignore<DomainEvent>();

            modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
            modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
            modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
            modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}