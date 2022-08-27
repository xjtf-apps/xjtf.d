namespace xjtf.d.dto
{
    public class ServiceDetailsPayload
    {
        [JsonPropertyName("serviceName")] public string ServiceName { get; set; }
        [JsonPropertyName("displayName")] public string? DisplayName { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("executablePath")] public string ExecutablePath { get; set; }
        [JsonPropertyName("arguments")] public string? Arguments { get; set; }


        [JsonPropertyName("debug")] public ServiceDebugPayload? Debug { get; set; }
        [JsonPropertyName("states")] public List<ServiceEntryStatePayload>? States { get; set; }
        [JsonPropertyName("observations")] public List<ServiceEntryActualPayload>? Observations { get; set; }
    }
}
