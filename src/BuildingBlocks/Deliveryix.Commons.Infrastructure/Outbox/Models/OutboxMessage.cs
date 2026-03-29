namespace Deliveryix.Commons.Infrastructure.Outbox.Models
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTimeOffset OccurredOn { get; set; }
        public DateTimeOffset? ProcessedOn { get; set; }
        public string? Error { get; set; }
    }
}