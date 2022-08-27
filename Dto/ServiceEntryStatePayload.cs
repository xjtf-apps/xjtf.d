namespace xjtf.d.dto
{
    public class ServiceEntryStatePayload
    {
        [JsonPropertyName("serviceEntryStateId")] public int ServiceEntryStateId { get; set; }
        [JsonPropertyName("daemonManaged")] public bool DaemonManaged { get; set; }
    }
}
