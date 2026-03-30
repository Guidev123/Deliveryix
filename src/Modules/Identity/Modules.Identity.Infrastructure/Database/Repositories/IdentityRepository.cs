using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Identities.Repositories;

namespace Modules.Identity.Infrastructure.Database.Repositories
{
    internal sealed class IdentityRepository(IdentityDbContext context) : IIdentityRepository
    {
        public Task<bool> ExistsAsync(string document, CancellationToken cancellationToken = default)
            => context.Identities.AnyAsync(c => c.Document.Number == document, cancellationToken);

        public void Insert(Domain.Identities.Entities.Identity identity)
        {
            foreach (var role in identity.Roles)
            {
                context.Attach(role);
            }

            context.Identities.Add(identity);
        }

        public async Task<bool> CommitAsync(CancellationToken cancellationToken)
            => await context.SaveChangesAsync(cancellationToken) > 0;
    }
}