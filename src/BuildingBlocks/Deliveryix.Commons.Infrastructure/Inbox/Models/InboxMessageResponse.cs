namespace Deliveryix.Commons.Infrastructure.Inbox.Models
{
    public sealed class InboxMessageResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
    }
}