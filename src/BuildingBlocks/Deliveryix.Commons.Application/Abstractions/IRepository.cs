using Deliveryix.Commons.Domain.DomainObjects;

namespace Deliveryix.Commons.Application.Abstractions
{
    public interface IRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot;
}