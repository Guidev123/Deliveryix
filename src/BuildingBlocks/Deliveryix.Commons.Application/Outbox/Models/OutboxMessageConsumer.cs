namespace Deliveryix.Commons.Application.Outbox.Models
{
    public sealed class OutboxMessageConsumer
    {
        public OutboxMessageConsumer(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}