namespace Deliveryix.Commons.Application.Outbox.Models
{
    public sealed class OutboxMessageResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
    }
}