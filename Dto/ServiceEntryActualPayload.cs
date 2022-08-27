namespace xjtf.d.dto
{
    public class ServiceEntryActualPayload
    {
        [JsonPropertyName("serviceEntryActualId")] public int ServiceEntryActualId { get; set; }
        [JsonPropertyName("observedOnUtc")] public int ObservedOnUtc { get; set; }
        [JsonPropertyName("status")] public string Status { get; set; }
    }
}
