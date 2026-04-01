using System.Text.Json.Serialization;

namespace Deliveryix.Commons.Application.Messaging
{
    public abstract record IntegrationEvent : IIntegrationEvent
    {
        [JsonConstructor]
        protected IntegrationEvent() { }

        protected IntegrationEvent(string messageType, string module)
        {
            CorrelationId = Guid.NewGuid();
            OccurredOn = DateTimeOffset.UtcNow;
            MessageType = messageType;
            Module = module;
        }

        public Guid CorrelationId { get; set; }

        public DateTimeOffset OccurredOn { get; set; }

        public string MessageType { get; set; } = null!;

        public string Module { get; set; } = null!;
    }
}