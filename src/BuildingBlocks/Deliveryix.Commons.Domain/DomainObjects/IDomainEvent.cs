using System.Text.Json.Serialization;

namespace Deliveryix.Commons.Domain.DomainObjects
{
    internal interface IDomainEvent : IEvent
    {
        Guid AggregateId { get; }
    }

    public abstract record DomainEvent : IDomainEvent
    {
        [JsonConstructor]
        protected DomainEvent() { }

        protected DomainEvent(Guid aggregateId, string messagetype)
        {
            AggregateId = aggregateId;
            CorrelationId = Guid.NewGuid();
            OccurredOn = DateTimeOffset.UtcNow;
            Messagetype = messagetype;
        }

        public Guid AggregateId { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTimeOffset OccurredOn { get; set; }
        public string Messagetype { get; set; } = null!;
    }
}