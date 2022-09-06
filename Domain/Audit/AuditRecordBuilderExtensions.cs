namespace xjtf.d;

public static class AuditRecordBuilderExtensions
{
    public static AuditRecordBuilder WithSubject(this AuditRecordBuilder builder, string subject)
    {
        builder._subject = subject;
        return builder;
    }

    public static AuditRecordBuilder WithVerb(this AuditRecordBuilder builder, string verb)
    {
        builder._verb = verb;
        return builder;
    }

    public static AuditRecordBuilder WithObject(this AuditRecordBuilder builder, string @object)
    {
        builder._object = @object;
        return builder;
    }

    public static AuditRecordBuilder WithEventKey(this AuditRecordBuilder builder, string? eventKey)
    {
        builder._eventKey = eventKey;
        return builder;
    }

    public static AuditRecord Build(this AuditRecordBuilder builder)
    {
        if (!builder.Valid)
            throw new NullReferenceException("Some of the audit record arguments were missing");

        return new AuditRecord()
        {
            Subject = builder._subject,
            Verb = builder._verb,
            Object = builder._object,
            EventKey = builder._eventKey
        };
    }
}
