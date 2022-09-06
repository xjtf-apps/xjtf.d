namespace xjtf.d;

public class AuditRecordBuilder
{
    internal string? _subject;
    internal string? _verb;
    internal string? _object;
    internal string? _eventKey;

    public AuditRecordBuilder() { }

    internal bool Valid =>
        !string.IsNullOrEmpty(_subject) &&
        !string.IsNullOrEmpty(_verb) &&
        !string.IsNullOrEmpty(_object);

}
