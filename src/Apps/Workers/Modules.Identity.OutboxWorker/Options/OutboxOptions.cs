namespace Modules.Identity.OutboxWorker.Options
{
    public sealed class OutboxOptions
    {
        public const string SectionName = "Outbox";

        public int IntervalInSeconds { get; set; }
        public int BatchSize { get; set; }
    }
}