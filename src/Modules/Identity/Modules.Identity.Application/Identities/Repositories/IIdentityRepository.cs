using Deliveryix.Commons.Application.Abstractions;

namespace Modules.Identity.Application.Identities.Repositories
{
    public interface IIdentityRepository : IRepository<Domain.Identities.Entities.Identity>
    {
        Task<Domain.Identities.Entities.Identity?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default);

        Task<Domain.Identities.Entities.Identity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(string document, CancellationToken cancellationToken = default);

        void Insert(Domain.Identities.Entities.Identity identity);
    }
}