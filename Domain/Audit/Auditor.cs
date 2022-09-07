namespace xjtf.d;

public static class Auditor
{
    // log entry
    public static void LogEntry(AuditLogWriter logWriter, AuditRecord record, [CallerMemberName]string? source = null)
    {
        logWriter.Queue(PrepareLogEntry(record, source));
    }

    public static void LogEntry(AuditLogWriter logWriter, Func<AuditRecord?> recordEffect, [CallerMemberName]string? source = null)
    {
        var logEntry = PrepareLogEntry(recordEffect, source);
        if (logEntry != null)
            logWriter.Queue(logEntry);
    }

    public static async Task LogEntryAsync(AuditLogWriter logWriter, Task<AuditRecord> recordTask, [CallerMemberName]string? source = null)
    {
        var logEntry = await PrepareLogEntryAsync(recordTask, source);
        if (logEntry != null)
            logWriter.Queue(logEntry);
    }

    // log entries
    public static void LogEntries(AuditLogWriter logWriter, IEnumerable<AuditRecord> records, [CallerMemberName]string? source = null)
    {
        logWriter.Queue(records.Select(r => PrepareLogEntry(r, source)).ToArray());
    }

    public static void LogEntries(AuditLogWriter logWriter, IEnumerable<Func<AuditRecord?>> recordEffects, [CallerMemberName]string? source = null)
    {
        logWriter.Queue(recordEffects.Select(effect => effect()).Where(record => record != null).Cast<AuditRecord>().Select(record => PrepareLogEntry(record, source)).ToArray());
    }

    public static async Task LogEntriesAsync(AuditLogWriter logWriter, IEnumerable<Task<AuditRecord>> recordTasks, [CallerMemberName]string? source = null)
    {
        logWriter.Queue(await PrepareLogEntriesAsync(recordTasks, source));
    }

    // prepare log entry
    public static AuditLogWriter.QueableItem PrepareLogEntry(AuditRecord record, [CallerMemberName]string? source = null)
    {
        return new AuditLogWriter.QueableItem()
        {
            AuditRecord = record,
            Source = source ?? "<Unknown source>"
        };
    }
    public static AuditLogWriter.QueableItem? PrepareLogEntry(Func<AuditRecord?> recordEffect, [CallerMemberName]string? source = null)
    {
        AuditRecord? record;
        try
        {
            record = recordEffect();
        }
        catch
        {
            record = null;
        }
        return record == null ? null : PrepareLogEntry(record, source);
    }

    public static async Task<AuditLogWriter.QueableItem?> PrepareLogEntryAsync(Task<AuditRecord> recordTask, [CallerMemberName]string? source = null)
    {
        var record = await recordTask;
        return record == null ? null : PrepareLogEntry(record, source);
    }

    // prepare log entries
    public static IEnumerable<AuditLogWriter.QueableItem> PrepareLogEntries(IEnumerable<AuditRecord> records, [CallerMemberName] string? source = null)
    {
        return records.Select(r => PrepareLogEntry(r, source));
    }

    public static IEnumerable<AuditLogWriter.QueableItem> PrepareLogEntries(IEnumerable<Func<AuditRecord?>> recordEffects, [CallerMemberName] string? source = null)
    {
        return recordEffects.Select(record => PrepareLogEntry(record, source)).Where(entry => entry != null).Cast<AuditLogWriter.QueableItem>();
    }

    public static async Task<AuditLogWriter.QueableItem[]> PrepareLogEntriesAsync(IEnumerable<Task<AuditRecord>> recordTasks, [CallerMemberName] string? source = null)
    {
        var entryTasks = recordTasks.Select(async recordTask => await PrepareLogEntryAsync(recordTask, source));
        return (await Task.WhenAll(entryTasks)).Where(entry => entry != null).Cast<AuditLogWriter.QueableItem>().ToArray();
    }
}