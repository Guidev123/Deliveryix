namespace Deliveryix.Commons.Domain.DomainObjects
{
    public interface IEvent
    {
        Guid CorrelationId { get; }
        DateTimeOffset OccurredOn { get; }
        string Messagetype { get; }
    }
}