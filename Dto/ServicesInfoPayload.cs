namespace xjtf.d.dto
{
    public class ServicesInfoPayload
    {
        [JsonPropertyName("services")] public List<object> Services { get; set; }
    }
}
