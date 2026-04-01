using Deliveryix.Commons.Domain.DomainObjects;
using Deliveryix.Commons.Infrastructure.Inbox.Configurations;
using Deliveryix.Commons.Infrastructure.Outbox.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ModuleExtensions = Modules.Identity.Domain.Identities.Extensions.ModuleExtensions;

namespace Modules.Identity.Infrastructure.Database
{
    internal sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> context) : DbContext(context)
    {
        public DbSet<Domain.Identities.Entities.Identity> Identities { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(ModuleExtensions.ModuleName);

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