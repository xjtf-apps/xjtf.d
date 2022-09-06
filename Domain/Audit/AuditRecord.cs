namespace xjtf.d;

public class AuditRecord
{
    internal AuditRecord() { }
    public string Subject { get; internal init; }
    public string Verb { get; internal init; }
    public string Object { get; internal init; }
    public string? EventKey { get; internal init; }
}
