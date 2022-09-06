namespace xjtf.d;

public sealed partial class ServiceStore
{
    private DirectoryInfo StorageRoot
    {
        get
        {
            var localFolderRef = Environment.SpecialFolder.LocalApplicationData;
            var localFolder = Environment.GetFolderPath(localFolderRef);
            var storePath = Path.Combine(localFolder, "xjtf.d", "Store");
            var root = new DirectoryInfo(storePath);

            if (!root.Exists)
                root.Create();

            return root;
        }
    }

    private DirectoryInfo this[string serviceName]
    {
        get
        {
            var root = StorageRoot.FullName;
            var store = Path.Combine(root, serviceName);
            return new DirectoryInfo(store);
        }
    }

    public bool ExistsStore(string serviceName)
        =>
            this[serviceName].ExistsStorageUnit();
    
    public void CreateStore(string serviceName)
        =>
            this[serviceName].CreateStorageUnit();

    public ServiceStorageUnit AccessStore(string servicename)
        =>
            new ServiceStorageUnit(this, servicename);
    
    public ServiceStoreDescriptor[] EnumerateStores()
    {
        var root = StorageRoot;
        var rootName = root.FullName;
        var rootEnumOptions = new EnumerationOptions() { RecurseSubdirectories = false };
        var storeEnumOptions = new EnumerationOptions() { RecurseSubdirectories = true };
        var rootStores = root.EnumerateDirectories("*", rootEnumOptions).Where(d => d.ExistsStorageUnit());

        return rootStores.Select(store => new ServiceStoreDescriptor()
        {
            StoreName = store.Name,
            StoreUniqueIdentifier = store.GetServiceTagValue()?.ToString() ?? Guid.Empty.ToString(),
            StoreFilesCount = store
                .EnumerateFiles("*", storeEnumOptions)
                .Where(ServiceStoreExtensions.IsNotServiceTag)
                .Count(),
            StoreSizeBytes = store
                .EnumerateFiles("*", storeEnumOptions)
                .Where(ServiceStoreExtensions.IsNotServiceTag)
                .Select(f => f.Length)
                .Aggregate((a, b) => a + b)
        })
        .ToArray();
    }
}
