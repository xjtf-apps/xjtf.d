namespace xjtf.d;

public static class ServiceStoreExtensions
{
    /// <summary>
    /// A constant name for the dotfile which identifies a folder as being a service store folder.
    /// </summary>
    public static readonly string ServiceTagFilename =
        ".service.tag";

    /// <summary>
    /// Checks that the file is not a service store dotfile.
    /// </summary>
    public static readonly Func<FileInfo,bool> IsNotServiceTag = (file)
        => file.Name != ServiceTagFilename;

    /// <summary>
    /// Checks that the fs entry is not a service store dotfile.
    /// </summary>
    public static readonly Func<FileSystemInfo, bool> IsNotServiceTag_ = (fsi)
        => fsi is DirectoryInfo || (fsi is FileInfo fi && IsNotServiceTag(fi));

    /// <summary>
    /// Tries to read the service store's unique id from the service dotfile.
    /// </summary>
    public static Guid? GetServiceTagValue(this DirectoryInfo serviceStore)
        => serviceStore.HasServiceTag()
            ? Guid.Parse(File.ReadAllText(serviceStore.GetServiceTagInfo().FullName)) : null;

    /// <summary>
    /// Checks if the service store folder exists and is uniquely identifieable.
    /// </summary>
    public static bool ExistsStorageUnit(this DirectoryInfo serviceStore)
        => serviceStore.GetServiceTagInfo().Exists;

    /// <summary>
    /// Creates a new directory to use as a service store and initializes it with a dotfile.
    /// </summary>
    public static void CreateStorageUnit(this DirectoryInfo serviceStore)
    {
        serviceStore.Create();
        serviceStore.CreateServiceTag();
    }

    /// <summary>
    /// Checks if the service store directory has a dotfile.
    /// </summary>
    internal static bool HasServiceTag(this DirectoryInfo serviceStore)
        => serviceStore.GetServiceTagInfo().Exists;
    
    private static FileInfo GetServiceTagInfo(this DirectoryInfo serviceStore)
        => new FileInfo(Path.Combine(serviceStore.FullName, ServiceTagFilename));

    private static void CreateServiceTag(this DirectoryInfo serviceStore)
        => File.WriteAllText(serviceStore.GetServiceTagInfo().FullName, Guid.NewGuid().ToString());
}