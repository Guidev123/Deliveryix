using Deliveryix.Commons.Application.Messaging;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Identities.Repositories;

namespace Modules.Identity.Infrastructure.Database.Repositories
{
    internal sealed class IdentityRepository(
        IdentityDbContext context,
        IDomainEventCollector domainEventCollector
        ) : IIdentityRepository
    {
        public Task<bool> ExistsAsync(string document, CancellationToken cancellationToken = default)
            => context.Identities.AsNoTracking().AnyAsync(c => c.Document.Number == document, cancellationToken);

        public Task<Domain.Identities.Entities.Identity?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default)
            => context.Identities.AsNoTracking().FirstOrDefaultAsync(c => c.Document.Number == document, cancellationToken);

        public Task<Domain.Identities.Entities.Identity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => context.Identities.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public void Insert(Domain.Identities.Entities.Identity identity)
        {
            foreach (var role in identity.Roles)
            {
                context.Attach(role);
            }

            context.Identities.Add(identity);
            domainEventCollector.Collect(identity);
        }
    }
}