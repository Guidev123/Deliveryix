using MidR.Interfaces;

namespace Deliveryix.Commons.Domain.DomainObjects
{
    public interface IEvent : INotification
    {
        Guid CorrelationId { get; }
        DateTimeOffset OccurredOn { get; }
        string MessageType { get; }
    }
}