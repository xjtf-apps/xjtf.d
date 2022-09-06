namespace xjtf.d;

public static class Auditor
{
    public static async Task<AuditLogWriter.QueableItem?> PrepareLogEntryAsync(Task<AuditRecord> recordTask, [CallerMemberName]string? source = null)
    {
        var record = await recordTask;
        return record == null ? null : PrepareLogEntry(record, source);
    }

    public static AuditLogWriter.QueableItem? PrepareLogEntry(Func<AuditRecord?> recordEffect, [CallerMemberName]string? source = null)
    {
        var record = null as AuditRecord;
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

    public static AuditLogWriter.QueableItem PrepareLogEntry(AuditRecord record, [CallerMemberName]string? source = null)
    {
        return new AuditLogWriter.QueableItem()
        {
            AuditRecord = record,
            Source = source ?? "<Unknown source>"
        };
    }
}