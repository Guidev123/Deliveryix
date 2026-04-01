namespace Deliveryix.Commons.Infrastructure.EventBus
{
    public sealed class ServiceBusOptions
    {
        public const string SectionName = "ServiceBus";

        public string FullyQualifiedNamespace { get; set; } = null!;
        public string ConnectionString { get; set; } = null!;
    }
}