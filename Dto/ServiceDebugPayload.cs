namespace xjtf.d.dto
{
    public class ServiceDebugPayload
    {
        [JsonPropertyName("installationError")] public string? InstallationError { get; set; }
        [JsonPropertyName("startError")] public string? StartError { get; set; }
    }
}
