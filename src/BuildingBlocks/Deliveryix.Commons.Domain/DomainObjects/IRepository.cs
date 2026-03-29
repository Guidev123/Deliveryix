namespace Deliveryix.Commons.Domain.DomainObjects
{
    public interface IRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot, new();
}