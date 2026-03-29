namespace Deliveryix.Commons.Infrastructure.Outbox.Models
{
    public sealed class OutboxMessageMessageConsumer
    {
        public OutboxMessageMessageConsumer(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}