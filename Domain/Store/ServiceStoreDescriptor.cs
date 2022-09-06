namespace xjtf.d;

public class ServiceStoreDescriptor
{
    [JsonPropertyName("storeName")] public string StoreName { get; set; }
    [JsonPropertyName("storeUniqueIdentifier")] public string StoreUniqueIdentifier { get; set; }
    [JsonPropertyName("storeFilesCount")] public int StoreFilesCount { get; set; }
    [JsonPropertyName("storeSizeBytes")] public long StoreSizeBytes { get; set; }
}
