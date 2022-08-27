namespace xjtf.d.dto
{
    public class ServiceEntryPayload
    {
        [JsonPropertyName("serviceEntryId")] public int? ServiceEntryId { get; set; }
        [JsonPropertyName("serviceName")] public string ServiceName { get; set; }
        [JsonPropertyName("displayName")] public string? DisplayName { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("executablePath")] public string ExecutablePath { get; set; }
        [JsonPropertyName("arguments")] public string? Arguments { get; set; }
    }
}
