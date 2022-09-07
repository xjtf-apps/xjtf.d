namespace xjtf.d;

public static class AuditVerbs
{
    public static class Storage
    {
        public const string ServiceUploaded = "SERVICE_UPLOADED";
        public const string ServiceDeleted = "SERVICE_DELETED";
    }

    public static class Host
    {
        public const string ServiceInstalled = "SERVICE_INSTALLED";
    }
}

public static class AuditEventKeys
{
    public static class Area
    {
        public const string Store = "STORE";
        public const string Host = "HOST";
    }
}
