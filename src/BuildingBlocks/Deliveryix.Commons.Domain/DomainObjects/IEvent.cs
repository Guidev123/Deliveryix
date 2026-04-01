using MidR.Interfaces;

namespace Deliveryix.Commons.Domain.DomainObjects
{
    public interface IEvent : INotification
    {
        Guid CorrelationId { get; }
        DateTimeOffset OccurredOn { get; }
        string Messagetype { get; }

        static T Create<T>(T @event) where T : IEvent => @event;
    }
}