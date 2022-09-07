namespace xjtf.d;

public sealed partial class ServiceStore
{
    public sealed class ServiceStorageUnit
    {
        private readonly DirectoryInfo _storage;
        internal string InstallFolder => _storage.FullName;
        public bool Empty => EnumerateContents().Count() == 0;

        internal ServiceStorageUnit(ServiceStore serviceStore, string serviceName)
        {
            if (!serviceStore.ExistsStore(serviceName))
                serviceStore.CreateStore(serviceName);

            _storage = serviceStore[serviceName];
        }

        public IEnumerable<string> EnumerateContents()
        {
            return _storage.EnumerateFiles("*", new EnumerationOptions()
            {
                RecurseSubdirectories = true
            })
            .Where(ServiceStoreExtensions.IsNotServiceTag)
            .Select(f => f.FullName.Replace($"{InstallFolder}{Path.DirectorySeparatorChar}", ""));
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
            _storage.CreateStorageUnit();
        }
    }
}