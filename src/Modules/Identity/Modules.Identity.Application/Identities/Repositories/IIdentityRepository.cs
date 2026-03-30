using Deliveryix.Commons.Domain.DomainObjects;

namespace Modules.Identity.Application.Identities.Repositories
{
    public interface IIdentityRepository : IRepository<Domain.Identities.Entities.Identity>
    {
        Task<bool> ExistsAsync(string document, CancellationToken cancellationToken = default);

        void Insert(Domain.Identities.Entities.Identity identity);
    }
}