using System.Text.Json.Serialization;

namespace Deliveryix.Commons.Domain.DomainObjects
{
    public abstract record DomainEvent : IDomainEvent
    {
        [JsonConstructor]
        protected DomainEvent() { }

        protected DomainEvent(Guid aggregateId, string messagetype, string module)
        {
            AggregateId = aggregateId;
            CorrelationId = Guid.NewGuid();
            OccurredOn = DateTimeOffset.UtcNow;
            Messagetype = messagetype;
            Module = module;
        }

        public Guid AggregateId { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTimeOffset OccurredOn { get; set; }
        public string Messagetype { get; set; } = null!;
        public string Module { get; set; } = null!;
    }
}