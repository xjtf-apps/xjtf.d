namespace xjtf.d;

public sealed class AuditLogWriter
{
    private AuditLogWriter() {}
    private static readonly ConcurrentQueue<QueableItem> _writeItems = new();
    internal static readonly AuditLogWriter Instance = new();
    internal void Queue(params QueableItem[] items)
    {
        items.AsParallel().ForAll(item =>
        {
            _writeItems.Enqueue(item);
        });
    }

    internal void Persist(DaemonDbContext dbContext, int n = 100)
    {
        if (n < 0)
            throw new ArgumentOutOfRangeException(nameof(n));

        var items =
            _writeItems.Take(n).ToArray().Select(item => new AuditLogEntry()
            {
                Subject = item.AuditRecord.Subject,
                Verb = item.AuditRecord.Verb,
                Object = item.AuditRecord.Object,
                EventKey = item.AuditRecord.EventKey,

                Created = item.Queued,
                Source = item.Source
            });

        dbContext.AuditLogEntries.AddRange(items);
        dbContext.SaveChanges();
    }

    public class QueableItem
    {
        public string Source { get; internal init; }
        public AuditRecord AuditRecord { get; internal init; }
        public readonly DateTimeOffset Queued = DateTimeOffset.Now;
    }
}