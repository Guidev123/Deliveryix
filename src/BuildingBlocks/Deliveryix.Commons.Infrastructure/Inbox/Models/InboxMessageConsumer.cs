namespace Deliveryix.Commons.Infrastructure.Inbox.Models
{
    public sealed class InboxMessageConsumer
    {
        public InboxMessageConsumer(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}