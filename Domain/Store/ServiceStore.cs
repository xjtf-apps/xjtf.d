namespace xjtf.d;

public class ServiceStore
{
    private DirectoryInfo StorageRoot
    {
        get
        {
            var localFolderRef = Environment.SpecialFolder.LocalApplicationData;
            var localFolder = Environment.GetFolderPath(localFolderRef);
            var storePath = Path.Combine(localFolder, "Store");
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

    public bool ExistsStore(string serviceName) => this[serviceName].Exists;
    public void CreateStore(string serviceName) => this[serviceName].Create();
    public StorageUnit AccessStore(string servicename) => new StorageUnit(this, servicename);

    public class StorageUnit
    {
        private readonly DirectoryInfo _storage;
        public string InstallFolder => _storage.FullName;
        public bool Empty => _storage.EnumerateFileSystemInfos().Count() > 0;

        internal StorageUnit(ServiceStore serviceStore, string serviceName)
        {
            if (!serviceStore.ExistsStore(serviceName))
                serviceStore.CreateStore(serviceName);

            _storage = serviceStore[serviceName];
        }

        public IEnumerable<string> EnumerateContents()
        {
            return _storage.EnumerateFiles("*", new EnumerationOptions() {
                RecurseSubdirectories = true
            })
            .Select(f => f.FullName);
        }

        public async Task AddFormFilesAsync(IEnumerable<IFormFile> formfiles)
        {
            await Task.WhenAll(formfiles.Select(ff => Task.Run(() => {
                var filename = ff.FileName;
                var filestream = ff.OpenReadStream();
                return AddFileAsync(filename, filestream);
            })));
        }

        public async Task AddFileAsync(string filename, Stream filestream)
        {
            var container = _storage;
            var separator = Path.DirectorySeparatorChar;
            var names = filename.Split(separator);
            var sname = names[^1];

            foreach (var parent in names[..^1])
            {
                var containerPath = Path.Combine(container.FullName, parent);
                container = new DirectoryInfo(containerPath);
                if (!container.Exists) container.Create();
            }

            using var writer = File.OpenWrite(filename);
            await filestream.CopyToAsync(writer);
        }

        public void Clear()
        {
            _storage.Delete(recursive: true);
            _storage.Create();
        }
    }
}